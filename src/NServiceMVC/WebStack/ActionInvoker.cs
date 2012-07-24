using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.Net.Mime;
using System.Web.Mvc;
using System.Globalization;
using System.Web.Mvc.Async;
using NServiceMVC.Formats;

namespace NServiceMVC.WebStack
{
    class ActionInvoker : ControllerActionInvoker
    {
        public ActionInvoker(ServiceController controller)
            : base()
        {
            this.Controller = controller;
        }

        protected ServiceController Controller { get; set; }

       // public string ViewName { get; set; }

        //public static Func<object, ActionResult> DefaultViewFunction { get; set; }

        private static readonly Type ActionResultType = typeof(ActionResult);

        protected override ActionResult CreateActionResult(ControllerContext controllerContext, ActionDescriptor actionDescriptor, object actionReturnValue)
        {
            // Inspect controller method's declared return type. If it's ActionResult or a derived type we bypass the NServiceMVC serialization logic.
            Type actionMethodReturnType = null;
            if (actionDescriptor is ReflectedActionDescriptor)
                actionMethodReturnType = ((ReflectedActionDescriptor)actionDescriptor).MethodInfo.ReturnType;
            else if (actionDescriptor is ReflectedAsyncActionDescriptor)
                actionMethodReturnType = ((ReflectedAsyncActionDescriptor)actionDescriptor).CompletedMethodInfo.ReturnType;

            if (actionMethodReturnType != null)
            {
                // Check if controller action method declares an ActionResult type then bypass the NServiceMVC logic and pass the result.
                if (ActionResultType.IsAssignableFrom(actionMethodReturnType))
                {
                    return (ActionResult)actionReturnValue;
                }
            }

            // bulk of this from ResourcesOverMvc.Web.Mvc.MultipleRepresentationsAttribute.OnActionExecuted()

            //// Create a list of the charsets the client is willing to support in order of preference
            //// (adding the encoding the client used in the request as the last alternative)
            //var charsetList = new CharsetList(controllerContext.HttpContext.Request.Headers["Accept-Charset"])
            //                        {new Charset(controllerContext.HttpContext.Request.ContentEncoding, 0.0001F)};
            
            ActionResult replacementResult = null;

            // Sort the accept types acceptable to the client into order of preference then look for
            // a response handler that supports one of the accept types.
            foreach (var contentTypeWrapper in GetAcceptHeaderContentTypes(Controller.RequestInfo.AcceptTypes))
            {
                //replacementResult = NServiceMVC.Formatter.TryCreateActionResult(ViewName, actionReturnValue, contentTypeWrapper.ContentType, charsetList);
                replacementResult = NServiceMVC.Formatter.CreateHttpResponse(contentTypeWrapper.ContentType.ToString(), actionReturnValue);

                if (replacementResult != null)
                {
                    return replacementResult;
                }
            }

            throw new HttpException((int)HttpStatusCode.NotAcceptable, "None of the formats specified in the accept header is supported.");
            
            //actionReturnValue = DefaultViewFunction.Invoke(actionReturnValue);
            //actionReturnValue = Controller.DefaultView(actionReturnValue);
            //return base.CreateActionResult(controllerContext, actionDescriptor, actionReturnValue);
        }


        #region Sorting of accept types

        internal static List<ContentTypeWrapper> GetAcceptHeaderContentTypes(string[] acceptHeaderElements)
        {
            var contentTypeWraperList = new List<ContentTypeWrapper>(acceptHeaderElements.Length);
            int index = 0;
            foreach (string acceptHeaderElement in acceptHeaderElements)
            {
                try
                {
                    var contentTypeWrapper = new ContentTypeWrapper(new ContentType(acceptHeaderElement), index);
                    contentTypeWraperList.Add(contentTypeWrapper);
                }
                catch (FormatException)
                {
                    // Ignore unknown formats to allow fallback
                }
                index++;
            }
            contentTypeWraperList.Sort(new AcceptHeaderElementComparer());
            return contentTypeWraperList;
        }

        internal class ContentTypeWrapper
        {
            public ContentType ContentType { get; set; }
            public int OriginalIndex { get; set; }

            public ContentTypeWrapper(ContentType contentType, int originalPosition)
            {
                ContentType = contentType;
                OriginalIndex = originalPosition;
            }
        }

        /// <summary>
        /// Can be used to sort a list of accept types into an order of preference. The "quality",
        /// specificity and original order are taken into account.
        /// </summary>
        class AcceptHeaderElementComparer : IComparer<ContentTypeWrapper>
        {
            public int Compare(ContentTypeWrapper x, ContentTypeWrapper y)
            {
                // Quality Factor is the over riding control of order
                var qualityDifference = GetQualityFactor(x.ContentType) - GetQualityFactor(y.ContentType);
                if (qualityDifference < 0)
                {
                    return 1;
                }
                if (qualityDifference > 0)
                {
                    return -1;
                }

                // Next control is which is the more specific
                string[] xTypeSubType = x.ContentType.MediaType.Split('/');
                string[] yTypeSubType = y.ContentType.MediaType.Split('/');

                if (string.Equals(xTypeSubType[0], yTypeSubType[0], StringComparison.OrdinalIgnoreCase))
                {
                    if (string.Equals(xTypeSubType[1], yTypeSubType[1], StringComparison.OrdinalIgnoreCase))
                    {
                        // need to check the number of parameters to determine which is more specific
                        int xNonQParameterCount = NonQParameterCount(x.ContentType);
                        int yNonQParameterCount = NonQParameterCount(y.ContentType);
                        if (xNonQParameterCount < yNonQParameterCount)
                        {
                            return 1;
                        }
                        if (xNonQParameterCount > yNonQParameterCount)
                        {
                            return -1;
                        }
                    }
                    else
                    {
                        if (xTypeSubType[1][0] == '*' && xTypeSubType[1].Length == 1)
                        {
                            return 1;
                        }
                        if (yTypeSubType[1][0] == '*' && yTypeSubType[1].Length == 1)
                        {
                            return -1;
                        }
                    }
                }

                // Lastly, if we still cannot distinguish we will assume the earliest in the list 
                // is the preferred type
                if (x.OriginalIndex > y.OriginalIndex)
                {
                    return 1;
                }
                if (x.OriginalIndex < y.OriginalIndex)
                {
                    return -1;
                }

                return 0;
            }

            static decimal GetQualityFactor(ContentType contentType)
            {
                if (contentType.Parameters != null)
                {
                    foreach (string key in contentType.Parameters.Keys.Cast<string>().Where(key => string.Equals("q", key, StringComparison.OrdinalIgnoreCase)))
                    {
                        decimal result;
                        if (decimal.TryParse(contentType.Parameters[key], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out result) && (result <= (decimal)1.0))
                        {
                            return result;
                        }
                    }
                }

                return (decimal)1.0;
            }

            static int NonQParameterCount(ContentType contentType)
            {
                var number = 0;
                if (contentType.Parameters != null)
                {
                    number = contentType.Parameters.Keys.Cast<string>().Count(param => !string.Equals("q", param, StringComparison.OrdinalIgnoreCase));
                }

                return number;
            }
        }

        #endregion
    }
}
