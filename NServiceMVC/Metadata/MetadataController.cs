using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace NServiceMVC.Metadata
{
    class MetadataController : Controller
    {
        public ActionResult Index()
        {
            return View("GenericObject");
        }
    }
}
