using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Net;

namespace NServiceMVC.WebStack
{
    /// <summary>
    /// An actionresult type that is basically a combination of <see cref="System.Web.Mvc.HttpStatusCodeResult"/> and <see cref="System.Web.Mvc.ContentResult"/>
    /// </summary>
    public class HttpStatusContentResult : ActionResult
    {
        /// <summary>
        /// Content with status 200 OK
        /// </summary>
        public HttpStatusContentResult()
            : this((int)HttpStatusCode.OK, (string)null)
        { }

        /// <summary>
        /// Content with a specific status code
        /// </summary>
        /// <param name="statusCode"></param>
        public HttpStatusContentResult(int statusCode)
            : this(statusCode, (string)null)
        { }

        /// <summary>
        /// Content with a specific status code and description
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="statusDescription"></param>
        public HttpStatusContentResult(int statusCode, string statusDescription)
            : this(statusCode, statusDescription, null)
        { }

        /// <summary>
        /// Wraps an existing content result with a specific status code
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="statusDescription"></param>
        /// <param name="content"></param>
        public HttpStatusContentResult(int statusCode, ContentResult content) 
            : this(statusCode, null, content) 
        { }
        

        /// <summary>
        /// Wraps an existing content result with a specific status code
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="statusDescription"></param>
        /// <param name="content"></param>
        public HttpStatusContentResult(int statusCode, string statusDescription, ContentResult content)
        {
            StatusCode = statusCode;
            StatusDescription = statusDescription;

            if (content != null)
            {
                Content = content.Content;
                ContentEncoding = content.ContentEncoding;
                ContentType = content.ContentType;
            }
        }




        public int StatusCode
        {
            get;
            private set;
        }

        public string StatusDescription
        {
            get;
            private set;
        }
        
        public string Content {
            get;
            set;
        }

        public Encoding ContentEncoding {
            get;
            set;
        }

        public string ContentType {
            get;
            set;
        }


        public override void ExecuteResult(ControllerContext context) {
            if (context == null) {
                throw new ArgumentNullException("context");
            }

            HttpResponseBase response = context.HttpContext.Response;

            context.HttpContext.Response.StatusCode = StatusCode;
            if (StatusDescription != null)
            {
                context.HttpContext.Response.StatusDescription = StatusDescription;
            }


            if (!String.IsNullOrEmpty(ContentType)) {
                response.ContentType = ContentType;
            }
            if (ContentEncoding != null) {
                response.ContentEncoding = ContentEncoding;
            }
            if (Content != null) {
                response.Write(Content);
            }
        }



    }
}
