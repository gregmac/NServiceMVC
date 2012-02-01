using System.Web.Mvc;
using System.Web.Routing;
using System.Reflection;

[assembly: WebActivator.PreApplicationStartMethod(typeof(NServiceMVC.Examples.HelloWorld.App_Start.NServiceMVCActivator), "Start")]

namespace NServiceMVC.Examples.HelloWorld.App_Start
{
    public static class NServiceMVCActivator
    {
        public static void Start()
        {
            NServiceMVC.Initialize(config =>
            {
                config.RegisterControllerAssembly(Assembly.GetExecutingAssembly());
                config.RegisterModelAssembly(Assembly.GetExecutingAssembly(), "NServiceMVC.Examples.HelloWorld.Models");
                config.Metadata();
            });
        }
    }
}
