﻿using System;
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

            return View(NServiceMVC.VirtualPathPrefix + "NServiceMVC.Metadata.Views.Index.cshtml", 
                new Models.MetadataSummary
                {
                    Routes = MetadataReflector.GetRouteDetails(),
                    Models = MetadataReflector.GetModelTypes(),
                }
            );

        }

        public ActionResult Test()
        {
            return View(NServiceMVC.VirtualPathPrefix + "NServiceMVC.Views.Index.cshtml", new { name = "test" });
        }
    }
}