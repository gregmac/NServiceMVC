﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.Net.Mime;


namespace NServiceMVC
{
    public class ServiceController : System.Web.Mvc.Controller
    {
        public ServiceController()
            : base()
        {
            ActionInvoker = new WebStack.ActionInvoker(this);
        }

        /// <summary>
        /// Information about the request. 
        /// </summary>
        public WebStack.HttpRequestInfo RequestInfo { get; set; }

        protected override void  OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        {
            RequestInfo = new WebStack.HttpRequestInfo(filterContext.HttpContext.Request);
            base.OnActionExecuting(filterContext);
        }

        protected override void  OnActionExecuted(System.Web.Mvc.ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }


    }
}
