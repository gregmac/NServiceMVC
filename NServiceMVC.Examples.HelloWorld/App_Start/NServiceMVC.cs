using System.Web.Mvc;
using System.Web.Routing;

[assembly: WebActivator.PreApplicationStartMethod(typeof(NServiceMVC.Examples.HelloWorld.App_Start.NServiceMVCActivator), "Start")]

namespace NServiceMVC.Examples.HelloWorld.App_Start
{
    public static class NServiceMVCActivator
    {
        public static void Start()
        {
            NServiceMVC.Initalize();
        }
    }
}
