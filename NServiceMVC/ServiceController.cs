using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NServiceMVC
{
    public class ServiceController : System.Web.Mvc.Controller 
    {
        public ServiceController()
            : base()
        {
            ActionInvoker = new ActionInvoker(this);
            
        }

        protected override void  OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        {   
 	
            base.OnActionExecuting(filterContext);
        }

        protected override void  OnActionExecuted(System.Web.Mvc.ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }

    }
}
