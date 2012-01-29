﻿////////////////////////////////////////////////////////////////////////////////////
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
using System.Web.Mvc;
using System.Net.Mime;
using System.IO;

namespace NServiceMVC.Formats.Xml
{
    public class XmlFormatHandler : IRequestFormatHandler, IResponseFormatHandler
    {
        public bool IgnoreMissingXslt { get; set; }
        public static string FriendlyName { get { return "Xml"; } }

        private static bool IsCompatibleMediaType(string mediaType)
        {
            return (mediaType == "text/xml" || mediaType == "application/xml");
        }

        public bool TryToMapFormatFriendlyName(string friendlyName, out string contentType)
        {
            if (string.Equals(friendlyName, FriendlyName, StringComparison.OrdinalIgnoreCase))
            {
                contentType = "application/xml";
                return true;
            }
            contentType = null;
            return false;
        }

        public ActionResult TryCreateActionResult(string viewName, object model, ContentType responseContentType, CharsetList acceptCharsetList)
        {
            XmlActionResult xmlActionResult = null;

            if (IsCompatibleMediaType(responseContentType.MediaType))
            {
                xmlActionResult = new XmlActionResult
                                      {
                                          IgnoreMissingXslt = IgnoreMissingXslt,
                                          XsltName = viewName,
                                          Data = model,
                                          AcceptCharsetList = acceptCharsetList
                                      };
            }

            return xmlActionResult;
        }

        public bool TryDeserializeRequestRepresentation(ControllerContext controllerContext, ModelBindingContext bindingContext, ContentType requestContentType, out object model)
        {
            bool result = false;
            model = null;

            if (IsCompatibleMediaType(requestContentType.MediaType))
            {
                var reader = new StreamReader(controllerContext.HttpContext.Request.InputStream, controllerContext.HttpContext.Request.ContentEncoding, true);
                string representation = reader.ReadToEnd();

                try
                {
                    var xsltSerializer = new XsltSerializer();
                    model = xsltSerializer.Deserialize(representation, controllerContext, bindingContext.ModelType.Name, bindingContext.ModelType, IgnoreMissingXslt);
                    result = true;
                }
                catch (Exception ex)
                {
                    string message = ex.Message;
                    if (ex.InnerException != null)
                    {
                        message = ex.InnerException.Message;
                    }

                    bindingContext.ModelState.AddModelError(bindingContext.ModelName, message);
                }

                var valueResult = new ValueProviderResult(representation, representation, System.Globalization.CultureInfo.InvariantCulture);
                bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueResult);
            }

            return result;
        }
    }
}