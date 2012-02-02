using System.Web.Mvc;
using System.Web.Routing;
using System.Reflection;
using NServiceMVC;

[assembly: WebActivator.PreApplicationStartMethod(typeof($rootnamespace$.App_Start.NServiceMVCActivator), "Start")]

namespace $rootnamespace$.App_Start
{
    public static class NServiceMVCActivator
    {
        public static void Start()
        {
            NServiceMVC.Initialize(config =>
            {
                // Register all controllers in this project (Remember, controllers must extend NServiceMVC.ServiceController!)
				config.RegisterControllerAssembly(Assembly.GetExecutingAssembly());
				
				// Register all models in the Models namespace of this project:
                config.RegisterModelAssembly(Assembly.GetExecutingAssembly(), "$rootnamespace$.Models");
				
				// expose metadata at ~/metadata
                config.Metadata("metadata");
            });
        }
    }
}
