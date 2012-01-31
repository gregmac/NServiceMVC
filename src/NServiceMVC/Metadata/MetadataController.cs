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
                new Models.MetadataSummary
                {
                    Routes = MetadataReflector.GetRouteDetails(),
                    Models = MetadataReflector.GetModelTypes(),
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

            return WebStack.TemplateEngine.RenderView("NServiceMVC.Metadata.Views.Op.Html", route);
        }

        public ActionResult Type(string id)
        {
            var type = MetadataReflector.GetModelTypes()[id];

            return WebStack.TemplateEngine.RenderView("NServiceMVC.Metadata.Views.Type.Html", type);
        }
    }
}
