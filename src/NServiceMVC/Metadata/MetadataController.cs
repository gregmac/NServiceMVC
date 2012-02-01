using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace NServiceMVC.Metadata
{
    public class MetadataController : Controller
    {
        public ActionResult Index()
        {
            return WebStack.TemplateEngine.RenderView("NServiceMVC.Metadata.Views.Index.html",
                new
                {
                    Model = new Models.MetadataSummary
                    {
                        Routes = MetadataReflector.GetRouteDetails(),
                        Models = MetadataReflector.GetModelTypes(),
                        Test = new List<string>(new string[] { "one", "two", "three"})
                    },
                    BaseUrl = NServiceMVC.Configuration.GetMetadataUrl(true)
                }
            );

        }

        /// <summary>
        /// View operation
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public ActionResult Op(string id)
        {
            //System.Web.Routing.RouteTable.Routes
            var route = (from r in MetadataReflector.GetRouteDetails()
                         where r.NiceUrl == id
                         select r).FirstOrDefault();

            return WebStack.TemplateEngine.RenderView("NServiceMVC.Metadata.Views.Op.html", 
                new
                {
                    Route = route,
                    BaseUrl = NServiceMVC.Configuration.GetMetadataUrl(true)
                }
            );
        }

        public ActionResult Type(string id)
        {
            var type = MetadataReflector.GetModelTypes()[id];

            return WebStack.TemplateEngine.RenderView("NServiceMVC.Metadata.Views.Type.html",
                new
                {
                    Model = type,
                    BaseUrl = NServiceMVC.Configuration.GetMetadataUrl(true)
                }
            );
        }
    }
}
