using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceMVC.Metadata.Models;
using System.Reflection;
using System.Text.RegularExpressions;


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

                var methods = from a in NServiceMVC.Configuration.ControllerAssemblies
                              from c in a.GetTypes()
                              from m in c.GetMethods()
                              where c.IsSubclassOf(typeof(ServiceController))
                              where m.GetCustomAttributes(typeof(AttributeRouting.RouteAttribute), true).Count() > 0
                              select m;

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
        public static ModelDetailCollection GetModelTypes()
        {
            if (_modelTypesCache == null)
            {
                List<ModelDetail> models = new List<ModelDetail>();

                var modelTypes = from a in NServiceMVC.Configuration.ModelAssemblies
                                 from c in a.Assembly.GetTypes()
                                 where string.IsNullOrEmpty(a.Namespace) || (a.Namespace == c.Namespace)
                                 select c;

                foreach (var model in modelTypes)
                {
                    var detail = CreateModelDetail(model, hasMetadata:true);
                    models.Add(detail);
                }

                _modelTypesCache = new ModelDetailCollection(models.OrderBy(x => x.Name));
            }
            return _modelTypesCache;
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
            var hasMetadata = (from t in GetModelTypes() where t.Name == type.FullName select true).FirstOrDefault();
            return CreateModelDetail(type, hasMetadata);
        }
        /// <summary>
        /// Creates a new modelDetail object
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static ModelDetail CreateModelDetail(System.Type type, bool hasMetadata)
        {
            var detail = new ModelDetail
            {
                Name = type.FullName,
                HasMetadata = hasMetadata,
            };

            var descriptionAttr = (System.ComponentModel.DescriptionAttribute)(type.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), true).FirstOrDefault());
            if (descriptionAttr != null) detail.Description = descriptionAttr.Description;

            object modelSample = Utilities.DefaultValueGenerator.GetDefaultValue(type);

            detail.SampleJson = Newtonsoft.Json.JsonConvert.SerializeObject(modelSample);

            return detail;
        }
    }
}
