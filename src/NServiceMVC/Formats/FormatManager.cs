using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NServiceMVC.Formats
{
    public class FormatManager
    {

        public FormatManager(NServiceMVC.NsConfiguration config)
        {
            if (config.AllowJson)
                JSON = new JsonFormatHandler();

            if (config.AllowXml)
                XML = new XmlFormatHandler();

            AllowXhtml = config.AllowXhtml;
        }

        protected bool AllowXhtml { get; private set; }

        #region Request handling
        public bool TryDeserializeModel(string input, Type modelType, out object model)
        {
            model = null;

            if (JSON != null)
            {
                try
                {
                    model = JSON.Deserialize(input, modelType);
                    return true;
                }
                catch { }
            }

            if (XML != null)
            {
                try
                {
                    model = XML.Deserialize(input, modelType);
                    return true;
                }
                catch { }
            }

            return false;
        }
        #endregion

        #region Response handling

        /// <summary>
        /// Tries to create a content result for the model using the specified contenttype.
        /// Returns null if it's not possible to create using the specified type.
        /// Normally, you should use <see cref="CreateHttpResponse"/>. 
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        internal System.Web.Mvc.ContentResult CreateContentResult(string contentType, object model)
        {
            switch (contentType.ToLower())
            {
                case "text/html":
                case "application/xhtml+xml":

                    if (AllowXhtml)
                    {
                        return new System.Web.Mvc.ContentResult
                        {
                            Content = (new Metadata.MetadataController()).XhtmlHelpPage(model),
                            ContentType = contentType,
                        };
                    }
                    break;

                case "application/json":
                case "application/x-javascript":
                case "text/javascript":
                case "text/x-javascript":
                case "text/x-json":
                case "text/json":
                    if (JSON != null)
                    {
                        return new System.Web.Mvc.ContentResult
                        {
                            Content = JSON.Serialize(model),
                            ContentType = contentType,
                        };
                    }
                    break;

                case "application/xml":
                case "text/xml":
                    if (XML != null)
                    {
                        return new System.Web.Mvc.ContentResult
                        {
                            Content = XML.Serialize(model),
                            ContentType = contentType,
                        };
                    }
                    break;
            }
            return null;
        }

        /// <summary>
        /// Creates an ActionResult for the model using the specified contenttype.
        /// May return an internal server error if there is a problem encoding,
        /// and may return HTTP not acceptable if the content type is unknown.
        /// </summary>
        /// <param name="contentType"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public System.Web.Mvc.ActionResult CreateHttpResponse(string contentType, object model)
        {
            contentType = GetContentTypeFromAlias(contentType);

            try
            {
                var response = CreateContentResult(contentType, model);
                if (response != null)
                {
                    return response;
                }
            }
            catch (Exception ex)
            {
                // TODO: return this as an actual error object in the given encoding?
                return new HttpServerError("Error encoding response: " + ex.Message);
            }

            // could not handle this contenttype
            return new HttpNotAcceptableResult();

            
        }

        public class HttpNotAcceptableResult : System.Web.Mvc.HttpStatusCodeResult
        {
            public HttpNotAcceptableResult() : base((int)System.Net.HttpStatusCode.NotAcceptable, "None of the formats specified in the accept header is supported.") { }
        }

        public class HttpServerError : System.Web.Mvc.HttpStatusCodeResult
        {
            public HttpServerError(string message) : base((int)System.Net.HttpStatusCode.InternalServerError, message) { }
        }


        /// <summary>
        /// Attempts to translate a mime-type or shortcut for a mime-type into a real mime-type we can use.
        /// Returns the original input if not matched.
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public static string GetContentTypeFromAlias(string alias)
        {
            if (string.IsNullOrEmpty(alias)) 
                return "text/html";

            switch (alias.ToLower())
            {
                case "json":
                    return "application/json"; // RFC 4627 section 6
                case "xml": 
                    return "text/xml"; // RFC 3023 section 3
                case "xhtml":
                    return "application/xhtml+xml";  // http://www.w3.org/TR/xhtml-media-types/#application-xhtml-xml
                case "help":
                case "html": 
                case "*/*":
                    return "text/html"; // http://www.w3.org/TR/xhtml-media-types/#media-types
                default:
                    return alias; // pass-through as-is
            }
        }

        #endregion

        #region Formats
        public IFormatHandler JSON { get; private set; }
        public IFormatHandler XML { get; private set; }
        #endregion
    }
}
