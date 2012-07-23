using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Web.Mvc;

namespace NServiceMVC.WebStack
{
    class NsExceptionFilter : IExceptionFilter 
    {

        public void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Controller is ServiceController)
            {
                ServiceController controller = (ServiceController)filterContext.Controller;

                object model;
                int httpStatusCode = (int)HttpStatusCode.InternalServerError;

                IServiceException serviceException = filterContext.Exception as IServiceException;
                if (serviceException!=null)
                {
                    model = serviceException.Model;
                    httpStatusCode = (int)serviceException.StatusCode;
                }
                else
                {
                    model = ExceptionToErrorModel(filterContext.Exception);
                }
            

                // Sort the accept types acceptable to the client into order of preference then look for
                // a response handler that supports one of the accept types.
                foreach (var contentTypeWrapper in ActionInvoker.GetAcceptHeaderContentTypes(controller.RequestInfo.AcceptTypes))
                {
                    var replacementResult = NServiceMVC.Formatter.CreateContentResult(contentTypeWrapper.ContentType.ToString(), model);

                    if (replacementResult != null)
                    {
                        filterContext.Result = new HttpStatusContentResult(httpStatusCode, replacementResult);
                        filterContext.ExceptionHandled = true;
                        return;
                    }
                }
            }
        }

        protected Metadata.Models.ErrorDetail ExceptionToErrorModel(Exception ex)
        {
            if (ex == null) return null;

            return new Metadata.Models.ErrorDetail
            {
                Message = ex.Message,
                StackTrace = ex.StackTrace,
                InnerError = ExceptionToErrorModel(ex.InnerException),  // recurse into inner errors
            };
        }

    }
}
