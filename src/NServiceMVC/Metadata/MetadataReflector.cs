using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceMVC.Metadata.Models;
using System.Reflection;
using System.Text.RegularExpressions;
using Utilities.Reflection.ExtensionMethods;


namespace NServiceMVC.Metadata
{
    class MetadataReflector
    {

        private static IEnumerable<RouteDetails> _routeDetailsCache;
        public static IEnumerable<RouteDetails> GetRouteDetails()
        {
            if (_routeDetailsCache == null)
            {
                var routeDetailsCache = new List<RouteDetails>();

                var methods = (from a in NServiceMVC.Configuration.ControllerAssemblies
                               from c in a.GetTypes()
                               from m in c.GetMethods()
                               where c.IsSubclassOf(typeof(ServiceController))
                               where m.GetCustomAttributes(typeof(AttributeRouting.RouteAttribute), true).Count() > 0
                               select m).Distinct();

                foreach (var method in methods)
                {
                    //todo: get return type, description

                    var descriptionAttr = (System.ComponentModel.DescriptionAttribute)(method.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), true).FirstOrDefault());
                    string description = string.Empty;
                    if (descriptionAttr != null) description = descriptionAttr.Description;


                    var routeAttrs = from a in method.GetCustomAttributes(typeof(AttributeRouting.RouteAttribute), true)
                                     select (AttributeRouting.RouteAttribute)a;

                    foreach (var routeAttr in routeAttrs)
                    {
                        var route = new RouteDetails
                        {
                            Url = routeAttr.Url,
                            Method = GetHttpMethod(routeAttr.HttpMethods),
                            Description = description,
                            ReturnType = CreateModelDetail(method.ReturnType),
                        };

                        // find names of parameters that exist in the URL
                        var urlParams = from m in (new Regex("\\{([a-zA-Z_][a-zA-Z0-9_]*)\\}")).Matches(routeAttr.Url).Cast<Match>()
                                        select m.Groups[1].Value;
                        var urls = urlParams.ToArray();

                        var actualParams = method.GetParameters();

                        // find the first parameter that doesn't exist in the URL -- this is our model
                        var modelParam = (from p in actualParams
                                          where !urlParams.Contains(p.Name)
                                          select p).FirstOrDefault();
                        if (modelParam != null)
                        {
                            route.ModelType = CreateModelDetail(modelParam.ParameterType);
                        }




                        var actualParamNames = from p in actualParams
                                               select p.Name;

                        route.Parameters =
                            (
                                from p in actualParams   // find parameter specified in the method and used in the URL
                                where urlParams.Contains(p.Name)
                                select new RouteDetails.ParameterDetails
                                {
                                    Name = p.Name,
                                    Type = p.ParameterType.Name,
                                    Default = p.DefaultValue.ToString(),
                                }
                            ).Union(
                                from name in urlParams   // find parameters specified in the URL, but not used by the actual method
                                where !actualParamNames.Contains(name)
                                select new RouteDetails.ParameterDetails
                                {
                                    Name = name,
                                    Type = "IGNORED",
                                    Default = string.Empty,
                                }
                            ).ToList();

                        routeDetailsCache.Add(route);

                    }

                }

                // order by URL, then method
                _routeDetailsCache = routeDetailsCache.OrderBy(x => x.Url + x.Method);
            }
            return _routeDetailsCache;
        }

        /// <summary>
        /// Extracts the HTTP method for the route attribute 
        /// </summary>
        /// <param name="methods"></param>
        /// <returns></returns>
        private static string GetHttpMethod(string[] methods)
        {
            if (methods.Contains("GET") && methods.Contains("HEAD") && methods.Count() == 2)
                return "GET";
            else if (methods.Count() == 1)
                return methods.First();
            else
                return String.Join(",", methods); // not really sure on this one..
        }

        #region GetModelTypes()
        private static ModelDetailCollection _modelTypesCache;
        /// <summary>
        /// Gets known model data types (cached). 
        /// </summary>
        /// <param name="includeBasicTypes">If the list should include basic types or not. 
        /// Basic types are type such as string, int that are in the System.* namspace.</param>
        /// <returns></returns>
        public static ModelDetailCollection GetModelTypes(bool includeBasicTypes = true)
        {
            if (_modelTypesCache == null)
            {
                List<ModelDetail> models = new List<ModelDetail>();

                var modelTypes = (from a in NServiceMVC.Configuration.ModelAssemblies
                                  from c in a.Assembly.GetTypes()
                                  where string.IsNullOrEmpty(a.Namespace) || (a.Namespace == c.Namespace)
                                  select c).Distinct();


                foreach (var model in modelTypes)
                {
                    var detail = CreateModelDetail(model, hasMetadata:true);
                    models.Add(detail);
                }

                // add basic types
                models.Add(CreateModelDetail(typeof(Int16), hasMetadata: true, defaultDescription: "16-bit signed integer"));
                models.Add(CreateModelDetail(typeof(Int32), hasMetadata: true, defaultDescription: "32-bit signed integer"));
                models.Add(CreateModelDetail(typeof(Int64), hasMetadata: true, defaultDescription: "64-bit signed integer"));
                models.Add(CreateModelDetail(typeof(UInt16), hasMetadata: true, defaultDescription: "16-bit unsigned integer"));
                models.Add(CreateModelDetail(typeof(UInt32), hasMetadata: true, defaultDescription: "32-bit unsigned integer"));
                models.Add(CreateModelDetail(typeof(UInt64), hasMetadata: true, defaultDescription: "64-bit unsigned integer"));
                models.Add(CreateModelDetail(typeof(String), hasMetadata: true, defaultDescription: "String (unicode supported)"));
                models.Add(CreateModelDetail(typeof(Boolean), hasMetadata: true, defaultDescription: "True/False value"));
                models.Add(CreateModelDetail(typeof(Single), hasMetadata: true, defaultDescription: "Single-precision floating-point number"));
                models.Add(CreateModelDetail(typeof(Double), hasMetadata: true, defaultDescription: "Double-precision floating-point number"));
                models.Add(CreateModelDetail(typeof(Decimal), hasMetadata: true, defaultDescription: "Decimal number"));
                models.Add(CreateModelDetail(typeof(Char), hasMetadata: true, defaultDescription: "Single character (unicode supported)"));
                models.Add(CreateModelDetail(typeof(DateTime), hasMetadata: true, defaultDescription: "Date and time"));
                models.Add(CreateModelDetail(typeof(TimeSpan), hasMetadata: true, defaultDescription: "Time interval"));
                models.Add(CreateModelDetail(typeof(Byte), hasMetadata: true, defaultDescription: "8-bit unsigned integer"));
                models.Add(CreateModelDetail(typeof(SByte), hasMetadata: true, defaultDescription: "8-bit signed integer"));


                _modelTypesCache = new ModelDetailCollection(models.OrderBy(x => x.Name));
            }

            if (includeBasicTypes)
                return _modelTypesCache;
            else
                return new ModelDetailCollection(from m in _modelTypesCache where m.IsBasicType == false select m);
        }
        #endregion

        /// <summary>
        /// Creates a new modelDetail object, and checks MetadataReflector.GetModelTypes() to see if 
        /// we have metadata
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static ModelDetail CreateModelDetail(System.Type type)
        {

            var hasMetadata = Utilities.DefaultValueGenerator.IsBasicType(type) 
                              || (from t in GetModelTypes() where t.Name == type.FullName select true).FirstOrDefault();
            return CreateModelDetail(type, hasMetadata);
        }
        /// <summary>
        /// Creates a new modelDetail object
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static ModelDetail CreateModelDetail(System.Type type, bool hasMetadata, string defaultDescription = null)
        {
            var detail = new ModelDetail
            {
                Name = type.GetName(),
                HasMetadata = hasMetadata,
                IsBasicType = Utilities.DefaultValueGenerator.IsBasicType(type),
                Description = defaultDescription,
            };

            var descriptionAttr = (System.ComponentModel.DescriptionAttribute)(type.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), true).FirstOrDefault());
            if (descriptionAttr != null) detail.Description = descriptionAttr.Description;

            object modelSample = Utilities.DefaultValueGenerator.GetSampleInstance(type); 
            
            if (NServiceMVC.Formatter.JSON != null)
            {
                try
                {
                    detail.SampleJson = NServiceMVC.Formatter.JSON.Serialize(modelSample, true);
                }
                catch (Exception ex)
                {
                    detail.SampleJson = "Error: " + ex.Message;
                }
            }

            if (NServiceMVC.Formatter.XML != null)
            {
                try
                {
                    detail.SampleXml = NServiceMVC.Formatter.XML.Serialize(modelSample, true);
                }
                catch (Exception ex)
                {
                    detail.SampleXml = "Error: " + ex.Message;
                }
            }

            detail.SampleCSharp = GetCSharpCode(type);

            return detail;
        }

                // from http://stackoverflow.com/questions/9104642/generate-source-code-for-class-definition-given-a-system-type/9104978#9104978
        private static string GetCSharpCode(Type t)
        {
            var sb = new StringBuilder();
            if (Utilities.DefaultValueGenerator.IsBasicType(t))
            {
                sb.AppendFormat("{0} {1};\n", t.GetName(), "value");
            }
            else
            {
                sb.AppendFormat("public class {0}\n{{\n", t.GetName());

                foreach (var field in t.GetFields())
                {
                    sb.AppendFormat("    public {0} {1};\n",
                        field.FieldType.Name,
                        field.Name);
                }

                foreach (var prop in t.GetProperties())
                {
                    sb.AppendFormat("    public {0} {1} {{{2}{3}}}\n",
                        prop.PropertyType.Name,
                        prop.Name,
                        prop.CanRead ? " get;" : "",
                        prop.CanWrite ? " set; " : " ");
                }

                sb.AppendLine("}");
            }
            
            return sb.ToString();
        } 
    }
}
