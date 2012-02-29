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
    /// <summary>
    /// Finds metadata related to the models and routes currently configured.
    /// </summary>
    public class MetadataReflector
    {
        /// <summary>
        /// Creates a new instance of the reflector
        /// </summary>
        /// <param name="config">The current NServiceMVC configuration</param>
        /// <param name="formatter">The current NServiceMVC FormatManager</param>
        public MetadataReflector(NServiceMVC.NsConfiguration config, Formats.FormatManager formatter)
        {
            Configuration = config;
            Formatter = formatter;

            // note: must initialize model before route
            ModelTypes = FindModelTypes();
            BasicModelTypes = FindBasicModelTypes();

            RouteDetails = FindRouteDetails();
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual IEnumerable<RouteDetails> RouteDetails { get; private set; }

        /// <summary>
        /// Registered, known model types
        /// </summary>
        public virtual ModelDetailCollection ModelTypes { get; private set; }

        /// <summary>
        /// Basic System.* types represented as models
        /// </summary>
        public virtual ModelDetailCollection BasicModelTypes { get; private set; }

        /// <summary>
        /// NServiceMVC configuration 
        /// </summary>
        public virtual NServiceMVC.NsConfiguration Configuration { get; private set; }

        /// <summary>
        /// NServiceMVC formatter
        /// </summary>
        public virtual Formats.FormatManager Formatter { get; private set; }

        /// <summary>
        /// Finds model details for a given type by its string name
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public ModelDetail FindModelDetail(string typeName)
        {
            bool isArray = false;
            System.Text.RegularExpressions.Match match;
            if ((match = System.Text.RegularExpressions.Regex.Match(typeName, "Array\\[(?<type>.*)\\]")).Success)
            {
                typeName = match.Groups["type"].Value;
                isArray = true;
            }

            Models.ModelDetail detail = null;
            if (ModelTypes.Contains(typeName))
                detail = ModelTypes[typeName];
            else if (BasicModelTypes.Contains(typeName))
                detail = BasicModelTypes[typeName];

            if ((detail != null) && isArray)
            {
                detail = GetModelDetailArray(detail.ModelType);
            }

            if (detail == null)    
                detail = new Models.ModelDetail()
                {
                    Name = "Unknown type",
                    Description = "The requested type is unknown",
                };

            return detail;
        }

        /// <summary>
        /// Create ModelDetail of an array of the passed type
        /// </summary>
        /// <param name="type">The element type of the array</param>
        /// <returns></returns>
        private ModelDetail GetModelDetailArray(Type type)
        {
            var arrayType = Array.CreateInstance(type, 1).GetType();
            return CreateModelDetail(arrayType, true, null);
        }

        private IEnumerable<RouteDetails> FindRouteDetails()
        {
            var routeDetailsCache = new List<RouteDetails>();

            var methods = (from a in Configuration.ControllerAssemblies
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
                        Url = routeAttr.RouteUrl,
                        Method = GetHttpMethod(routeAttr.HttpMethods),
                        Description = description,
                        ReturnType = CreateModelDetail(method.ReturnType),
                    };

                    // find names of parameters that exist in the URL
                    var urlParams = from m in (new Regex("\\{([a-zA-Z_][a-zA-Z0-9_]*)\\}")).Matches(routeAttr.RouteUrl).Cast<Match>()
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
                            from p in actualParams   // find parameter specified in the method 
                            select new RouteDetails.ParameterDetails
                            {
                                Name = p.Name,
                                Type = p.ParameterType.Name,
                                Default = p.DefaultValue.ToString(),
                                InUrl = urlParams.Contains(p.Name),
                            }
                        ).Union(
                            from name in urlParams   // find parameters specified in the URL, but not used by the actual method
                            where !actualParamNames.Contains(name)
                            select new RouteDetails.ParameterDetails
                            {
                                Name = name,
                                Type = "IGNORED",
                                Default = string.Empty,
                                InUrl = true,
                            }
                        ).ToList();

                    routeDetailsCache.Add(route);

                }

            }

            // order by URL, then method
            return routeDetailsCache.OrderBy(x => x.Url + x.Method);
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
        /// <summary>
        /// Gets known model data types (cached). 
        /// </summary>
        /// <returns></returns>
        private ModelDetailCollection FindModelTypes()
        {
            List<ModelDetail> models = new List<ModelDetail>();

            var modelTypes = (from a in Configuration.ModelAssemblies
                                from c in a.Assembly.GetTypes()
                                where string.IsNullOrEmpty(a.Namespace) || (a.Namespace == c.Namespace)
                                select c).Distinct();


            foreach (var model in modelTypes)
            {
                var detail = CreateModelDetail(model, hasMetadata:true);
                models.Add(detail);
            }


            return new ModelDetailCollection(models.OrderBy(x => x.Name));
        }

        /// <summary>
        /// Finds built-in System.* types, repesentign them as models
        /// </summary>
        /// <returns></returns>
        private ModelDetailCollection FindBasicModelTypes()
        {
            List<ModelDetail> models = new List<ModelDetail>();

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

            return new ModelDetailCollection(models);
        }
        #endregion

        /// <summary>
        /// Creates a new modelDetail object, and checks MetadataReflector.GetModelTypes() to see if 
        /// we have metadata
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private ModelDetail CreateModelDetail(System.Type type)
        {
            if (ModelTypes == null)
                throw new InvalidOperationException("Models must be initialized before routes");

            var hasMetadata = Utilities.DefaultValueGenerator.IsBasicType(type) 
                              || (from t in ModelTypes where t.Name == type.FullName select true).FirstOrDefault();
            return CreateModelDetail(type, hasMetadata);
        }
        /// <summary>
        /// Creates a new modelDetail object
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private ModelDetail CreateModelDetail(System.Type type, bool hasMetadata, string defaultDescription = null)
        {
            var detail = new ModelDetail
            {
                Name = type.GetName(),
                HasMetadata = hasMetadata,
                IsBasicType = Utilities.DefaultValueGenerator.IsBasicType(type),
                Description = defaultDescription,
                ModelType = type,
            };

            if (IsArrayOrEnumerableSingle(type))
            {
                var innerType = type.IsArray ? type.GetElementType() : type.GetGenericArguments()[0];

                detail.ArrayDetail = CreateModelDetail(innerType);

                detail.IsArray = true;

                detail.Name = string.Format("Array[{0}]", innerType.GetName());
            }

            var descriptionAttr = (System.ComponentModel.DescriptionAttribute)(type.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), true).FirstOrDefault());
            if (descriptionAttr != null) detail.Description = descriptionAttr.Description;

            try
            {
                object modelSample = Utilities.DefaultValueGenerator.GetSampleInstance(type);

                if (Formatter.JSON != null)
                {
                    try
                    {
                        detail.SampleJson = Formatter.JSON.Serialize(modelSample, true);
                    }
                    catch (Exception ex)
                    {
                        detail.SampleJson = "Error: " + ex.Message;
                    }
                }

                if (Formatter.XML != null)
                {
                    try
                    {
                        detail.SampleXml = Formatter.XML.Serialize(modelSample, true);
                    }
                    catch (Exception ex)
                    {
                        detail.SampleXml = "Error: " + ex.Message;
                    }
                }

                try
                {
                    detail.SampleCSharp = GetCSharpCode(type);
                }
                catch (Exception ex)
                {
                    detail.SampleCSharp = "Error: " + ex.Message;
                }


            }
            catch (Exception ex)
            {
                // todo: log error generating sample

                detail.SampleJson = "Error Creating Sample: " + ex.Message;
                detail.SampleXml = "Error Creating Sample: " + ex.Message;
                detail.SampleCSharp = "Error Creating Sample: " + ex.ToString();
            }


            return detail;
        }

        /// <summary>
        /// Checks if the type is an array or Enumerable (single-element arrays/lists only: not dictionaries/hashes)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsArrayOrEnumerableSingle(Type type)
        {
            return type.IsArray || (type.IsIEnumerable() && type.GetGenericArguments().Count() == 1);
        }


                // from http://stackoverflow.com/questions/9104642/generate-source-code-for-class-definition-given-a-system-type/9104978#9104978
        private static string GetCSharpCode(Type t)
        {
            var sb = new StringBuilder();
            if (Utilities.DefaultValueGenerator.IsBasicType(t) || IsArrayOrEnumerableSingle(t))
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
