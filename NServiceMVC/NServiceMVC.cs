using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Reflection;
using System.Web.Routing;

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
            
            Configuration = new NsConfiguration();
            // register the assembly that called this one
            Configuration.RegisterControllerAssembly(Assembly.GetCallingAssembly());

            if (config != null) config.Invoke(Configuration);
        }

        public static NsConfiguration Configuration { get; private set; }

        public class NsConfiguration
        {
            public NsConfiguration()
            {
                ControllerAssemblies = new List<System.Reflection.Assembly>();
                ModelAssemblies = new List<ModelAssembly>();
            }


            #region Controllers
            /// <summary>
            /// The list of assemblies containing controller methods to be included in 
            /// metadata output. Controllers must be inherited from <see cref="ServiceController"/>.
            /// </summary>
            /// <param name="assembly"></param>
            public List<Assembly> ControllerAssemblies { get; private set; }

            /// <summary>
            /// Register an assembly as containing controller methods to be included in 
            /// metadata output. Controllers must be inherited from <see cref="ServiceController"/>.
            /// </summary>
            /// <param name="assembly"></param>
            public void RegisterControllerAssembly(Assembly assembly)
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
            public List<ModelAssembly> ModelAssemblies { get; private set; }
            
            /// <summary>
            /// Registers all types in the givven assembly as models
            /// </summary>
            /// <param name="assembly"></param>
            public void RegisterModelAssembly(Assembly assembly)
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
            public void Metadata(string baseUrl = "metadata")
            {
                baseUrl = baseUrl.TrimEnd('/');

                RouteTable.Routes.Add(new System.Web.Routing.Route(baseUrl, new MvcRouteHandler()) {
                    Defaults = new RouteValueDictionary(new { controller = "metadata", action = "Index" })
                });
                   
                RouteTable.Routes.Add(new System.Web.Routing.Route(baseUrl + "/{action}/{*id}",new MvcRouteHandler()) {
                    Defaults = new RouteValueDictionary(new { controller = "metadata" })
                });
                    
            }
            #endregion

        }
    }
}
