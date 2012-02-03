////////////////////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2008 Piers Lawson
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in the
// Software without restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
// Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN
// AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.ObjectModel;
using System.Net.Mime;
using System.Web.Mvc;
using System.Web;
using System.Net;
using System.Linq;
using NServiceMVC.Formats.Xml;
using NServiceMVC.Formats.Json;
using NServiceMVC.Formats.Xhtml;

namespace NServiceMVC.Formats
{
    /// <summary>
    /// Holds list of Format Handlers that are used to translate incoming representations and
    /// create outgoing representations in the correct format.
    /// </summary>
    public class FormatManager
    {
        public FormatManager() 
        {
            RequestFormatHandlers = new RequestFormatHandlerCollection();
            ResponseFormatHandlers = new ResponseFormatHandlerCollection();

            if (NServiceMVC.Configuration.AllowJson) 
            {
                var xhtmlFormatHandler = new XhtmlFormatHandler { IgnoreMissingXslt = true };
                RequestFormatHandlers.Add(xhtmlFormatHandler);
                ResponseFormatHandlers.Add(xhtmlFormatHandler);
            }

            if (NServiceMVC.Configuration.AllowXml)
            {
                var xmlHandler = new XmlFormatHandler { IgnoreMissingXslt = true };
                RequestFormatHandlers.Add(xmlHandler);
                ResponseFormatHandlers.Add(xmlHandler);
            }

            if (NServiceMVC.Configuration.AllowJson)
            {
                var jsonHandler = new JsonNetFormatHandler();
                jsonHandler.JsonSerializerSettings = JsonNetSerializerSettings.CreateResponseSettings();
                jsonHandler.Formatting = Newtonsoft.Json.Formatting.None;

                RequestFormatHandlers.Add(jsonHandler);
                ResponseFormatHandlers.Add(jsonHandler);
            }
        }


        public RequestFormatHandlerCollection RequestFormatHandlers { get; private set; }
        public ResponseFormatHandlerCollection ResponseFormatHandlers { get; private set; }

        /// <summary>
        /// Asks each registered Format Handler (Response handlers followed by Request handlers) 
        /// if they understand the friendly name and can translate it into an explicit content type,
        /// until one responds positively.
        /// </summary>
        /// <param name="formatFriendlyName">Friendly name to translate</param>
        /// <param name="contentType">Explicit content type</param>
        /// <returns>Indicates if the friendly name was translated</returns>
        public bool TryToMapFormatFriendlyName(string formatFriendlyName, out string contentType)
        {
            foreach (IResponseFormatHandler responseFormatHandler in ResponseFormatHandlers)
            {
                if (responseFormatHandler.TryToMapFormatFriendlyName(formatFriendlyName, out contentType))
                {
                    return true;
                }
            }

            foreach (IRequestFormatHandler requestFormatHandler in RequestFormatHandlers)
            {
                if (requestFormatHandler.TryToMapFormatFriendlyName(formatFriendlyName, out contentType))
                {
                    return true;
                }
            }

            contentType = null;

            return false;
        }

        /// <summary>
        /// Asks each registered Response Format Handler if they can create an Action Result able to generate
        /// an outgoing representation of a specific content type, until one responds positively.
        /// </summary>
        /// <param name="name">The name of the view to create</param>
        /// <param name="model">Model data for the view</param>
        /// <param name="contentType">Content type to be generated by the view</param>
        /// <param name="acceptCharsetList">Encodings that are acceptable to the client</param>
        /// <returns>An Action Result capable of creating a representation of the correct content type</returns>
        public ActionResult TryCreateActionResult(string name, object model, ContentType contentType, CharsetList acceptCharsetList)
        {
            ActionResult result = null;

            foreach (IResponseFormatHandler potentialResponseFormatHandler in ResponseFormatHandlers)
            {
                result = potentialResponseFormatHandler.TryCreateActionResult(name, model, contentType, acceptCharsetList);

                if (result != null)
                {
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Asks each registered Request Format Handler if they can deserialize the request content.
        /// </summary>
        /// <param name="controllerContext">Context of the controller to which the data will be sent</param>
        /// <param name="bindingContext">Context of the binding required to bind to a particular parameter on the controller</param>
        /// <param name="model">Model to which the data should be added</param>
        /// <returns>Indicates if the data was bound</returns>
        public bool TryDeserializeRequestRepresentation(ControllerContext controllerContext, ModelBindingContext bindingContext, out object model)
        {
            bool result = false;
            model = null;

            string requestFormat = controllerContext.HttpContext.Request.ContentType;

            // Load the request's content type into a ContentType object. This should detect
            // unsupported content types and split out charset (i.e. encoding) information
            ContentType requestContentType = null;
            try
            {
                requestContentType = new ContentType(requestFormat);
            }
            catch (FormatException)
            {
                HandleContentTypeException(requestFormat);
            }
            catch (ArgumentNullException)
            {
                HandleContentTypeException(requestFormat);
            }
            catch (ArgumentException)
            {
                HandleContentTypeException(requestFormat);
            }

            if (requestContentType != null)
            {
                foreach (IRequestFormatHandler potentialRequestFormatHandler in RequestFormatHandlers)
                {
                    if (potentialRequestFormatHandler.TryDeserializeRequestRepresentation(controllerContext, bindingContext, requestContentType, out model))
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        private static void HandleContentTypeException(string requestFormat)
        {
            throw new HttpException((int)HttpStatusCode.UnsupportedMediaType, "Unsupported Media Type: '" + (string.IsNullOrEmpty(requestFormat) ? "(blank)" : requestFormat) + "'");
        }

        /// <summary>
        /// Register the standard format handlers (XHTML, XML and JSON)
        /// </summary>
        private class DefaultFormatManager : FormatManager
        {
            
        }


        public class RequestFormatHandlerCollection : System.Collections.ObjectModel.KeyedCollection<string, IRequestFormatHandler>
        {
            public RequestFormatHandlerCollection() : base() { }
            //public RequestFormatHandlerCollection(System.Collections.Generic.IEnumerable<IResponseFormatter> list)
            //    : base()
            //{
            //    if (list != null) foreach (var item in list) Add(item);
            //}

            protected override string GetKeyForItem(IRequestFormatHandler item)
            {
                return item.FriendlyName;
            }
        }

        public class ResponseFormatHandlerCollection : System.Collections.ObjectModel.KeyedCollection<string, IResponseFormatHandler>
        {
            public ResponseFormatHandlerCollection() : base() { }
            //public RequestFormatHandlerCollection(System.Collections.Generic.IEnumerable<IResponseFormatter> list)
            //    : base()
            //{
            //    if (list != null) foreach (var item in list) Add(item);
            //}

            protected override string GetKeyForItem(IResponseFormatHandler item)
            {
                return item.FriendlyName;
            }
        }

    }
}
