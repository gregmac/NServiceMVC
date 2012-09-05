﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Reflection;
using System.Web.Routing;
using Newtonsoft.Json;

namespace NServiceMVC
{
    abstract public class NServiceMVC
    {
        /// <summary>
        /// Bootstrap method for NServiceMVC
        /// </summary>
        public static void Initialize(Action<NsConfiguration> config)
        {
            ModelBinders.Binders.DefaultBinder = (new WebStack.MultipleRepresentationsBinder());

            WebStack.TemplateEngine.Initialize();

            // register our content handler
            RouteTable.Routes.Add(new System.Web.Routing.Route("__NServiceMvcContent/content/{*filename}", new MvcRouteHandler())
            {
                Defaults = new RouteValueDictionary(new { controller = "NServiceMvcContent", action = "File" })
            });

            // add global exception handler
            GlobalFilters.Filters.Add(new WebStack.NsExceptionFilter());


            Configuration = new NsConfiguration(Assembly.GetCallingAssembly());
            // register the assembly that called this one
            Configuration.RegisterControllerAssembly(Assembly.GetCallingAssembly());

            if (config != null) config.Invoke(Configuration);


            Formatter = new Formats.FormatManager(Configuration);
        }

        #region URLs

        /// <summary>
        /// Gets a URL to the NServiceMVC static content provider
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetContentUrl(string filename = "")
        {
            return GetBaseUrl("__NServiceMvcContent/content/" + filename);
        }

        /// <summary>
        /// Gets the base web application URL 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetBaseUrl(string path = "")
        {
            return UrlHelper.GenerateContentUrl("~/" + path, new System.Web.HttpContextWrapper(System.Web.HttpContext.Current));
        }

        /// <summary>
        /// Gets the metadata URL. 
        /// </summary>
        /// <param name="fullPath">If true, gets the full URL path. If false, just returns the url relative to the virtual application root</param>
        /// <returns></returns>
        public static string GetMetadataUrl(string path = "")
        {
            return GetBaseUrl(Configuration.MetadataUrl + "/" + path);
        }

        #endregion



        public static NsConfiguration Configuration { get; private set; }

        public static Formats.FormatManager Formatter { get; private set; }



        /// <summary>
        /// NServiceMVC configuration
        /// </summary>
        public class NsConfiguration
        {
            /// <summary>
            /// Creates a new instance of configuration. ApplicationTitle is set to the calling assembly name
            /// </summary>
            public NsConfiguration(Assembly callingAssembly)
                : this()
            {
                ApplicationTitle = callingAssembly.GetName().Name;
            }

            /// <summary>
            /// Creates a new instance of configuration. ApplicationTitle is not set.
            /// </summary>
            public NsConfiguration()
            {
                ControllerAssemblies = new List<System.Reflection.Assembly>();
                ModelAssemblies = new List<ModelAssembly>();

                AllowJson = true;
                AllowXhtml = true;
                AllowXml = true;

                JsonTypeNameHandling = TypeNameHandling.Auto;
            }

            public string ApplicationTitle { get; set; }

            #region Controllers
            /// <summary>
            /// The list of assemblies containing controller methods to be included in 
            /// metadata output. Controllers must be inherited from <see cref="ServiceController"/>.
            /// </summary>
            /// <param name="assembly"></param>
            public virtual List<Assembly> ControllerAssemblies { get; private set; }

            /// <summary>
            /// Register an assembly as containing controller methods to be included in 
            /// metadata output. Controllers must be inherited from <see cref="ServiceController"/>.
            /// </summary>
            /// <param name="assembly"></param>
            public virtual void RegisterControllerAssembly(Assembly assembly)
            {
                ControllerAssemblies.Add(assembly);
            }
            #endregion

            #region Models

            public class ModelAssembly
            {
                public Assembly Assembly { get; set; }
                public string Namespace { get; set; }
            }

            /// <summary>
            /// The list of assemblies and namespaces containing models which
            /// will be included in metadata information.
            /// </summary>
            public virtual List<ModelAssembly> ModelAssemblies { get; private set; }

            /// <summary>
            /// Registers all types in the givven assembly as models
            /// </summary>
            /// <param name="assembly"></param>
            public virtual void RegisterModelAssembly(Assembly assembly)
            {
                RegisterModelAssembly(assembly, null);
            }

            /// <summary>
            /// Reegisters all types in the given assembly with the specified namespace as models to
            /// be included in metadata output.
            /// This method can be called multiple times to register different namespaces within the same assembly.
            /// </summary>
            /// <param name="assembly"></param>
            /// <param name="namespace">The full (case-sensitive) namespace to look in</param>
            public void RegisterModelAssembly(Assembly assembly, string @namespace)
            {
                ModelAssemblies.Add(new ModelAssembly { Assembly = assembly, Namespace = @namespace });
            }
            #endregion

            #region Metadata

            /// <summary>
            /// The path (relative to virtual app dir) to the metadata controller
            /// </summary>
            public virtual string MetadataUrl { get; private set; }

            /// <summary>
            /// Set the location where metadata is served from
            /// </summary>
            /// <param name="baseUrl"></param>
            public virtual void Metadata(string baseUrl = "metadata")
            {
                MetadataUrl = baseUrl.TrimEnd('/').TrimStart(new char[] { '~', '/' });

                RouteTable.Routes.Add(new System.Web.Routing.Route(MetadataUrl, new MvcRouteHandler())
                {
                    Defaults = new RouteValueDictionary(new { controller = "metadata", action = "Index" })
                });

                RouteTable.Routes.Add(new System.Web.Routing.Route(MetadataUrl + "/{action}/{*id}", new MvcRouteHandler())
                {
                    Defaults = new RouteValueDictionary(new { controller = "metadata" })
                });

            }
            #endregion


            public virtual bool AllowJson { get; set; }
            public virtual bool AllowXml { get; set; }
            public virtual bool AllowXhtml { get; set; }

            /// <summary>
            /// If JSON is converted to camelCase (otherwise left as-is)
            /// </summary>
            public virtual bool JsonCamelCase { get; set; }

            /// <summary>
            /// If and how .NET types should be encoded in JSON objects 
            /// </summary>
            public virtual Newtonsoft.Json.TypeNameHandling JsonTypeNameHandling { get; set; }
        }
    }
}
