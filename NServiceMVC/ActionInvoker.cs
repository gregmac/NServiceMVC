using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace NServiceMVC
{
    class ActionInvoker : ControllerActionInvoker
    {
        public ActionInvoker(ServiceController controller)
            : base()
        {
            this.Controller = controller;
        }

        protected ServiceController Controller { get; set; }

        //public static Func<object, ActionResult> DefaultViewFunction { get; set; }

        protected override ActionResult CreateActionResult(ControllerContext controllerContext, ActionDescriptor actionDescriptor, object actionReturnValue)
        {
            //actionReturnValue = DefaultViewFunction.Invoke(actionReturnValue);
            //actionReturnValue = Controller.DefaultView(actionReturnValue);
            return base.CreateActionResult(controllerContext, actionDescriptor, actionReturnValue);
        }
    }
}
