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
using System.Net;
using System.Net.Mime;
using System.Web;

namespace NServiceMVC.WebStack
{
    /// <summary>
    /// Wraps a HttpRequest and intercepts both Http Verb and Content Type properties
    /// to allow for overloading of POST and supplying format information on the URI
    /// </summary>
    public class HttpRequestInfo
    {
        public const string XHttpMethodOverrideHeader = "X-Http-Method-Override";
        public static string HttpMethodOverrideFormId = XHttpMethodOverrideHeader;
        public static string HttpMethodOverrideQueryStringId = XHttpMethodOverrideHeader;
        public static string ContentTypeQueryStringId = "format"; // TODO: Extract to NServiceMVC.Configuration

        // ContentType defaults to "application/octet-stream" per RFC 2616 7.2.1
        public static string DefaultContentType = new ContentType().ToString();

        public const string UrlEncoded = "application/x-www-form-urlencoded";

        private readonly HttpRequestBase _httpRequest;
        private string _effectiveMethod;
        private string _effectiveContentType;

        public HttpRequestInfo(HttpRequestBase httpRequest)
        {
            _httpRequest = httpRequest;
        }

        /// <summary>
        /// Gets the HTTP data transfer method (such as GET, POST or HEAD) used by the client. This version
        /// allows the method to be overridden by the client, according to the following
        /// rules:
        /// 1. If the headers contain an override, its value is returned as the method
        /// 2. Otherwise, if the request is FORM based, look for a form variable that dictates the method
        /// 3. Otherwise, check for a variable in the querystring
        /// </summary>
        /// <returns>The method used by the client</returns>
        public  string HttpMethod
        {
            get
            {
                if (string.IsNullOrEmpty(_effectiveMethod))
                {
                    _effectiveMethod = GetHttpMethod(_httpRequest);
                }

                return _effectiveMethod;
            }
        }

        /// <summary>
        /// Gets or sets the MIME content type of the request. This version
        /// allows the accept types to be overridden according to the following
        /// rules:
        /// 1. If present return the Content Type provided in the request header
        /// 2. Otherwise, look for a content type within the querystring information
        /// 3. Otherwise (as per RFC 2616 7.2.1.) default to "application/octet-stream"
        /// </summary>
        public  string ContentType
        {
            get
            {
                string contentType = _effectiveContentType;

               
                // RFC 2616 7.2.1 which allows for inspection of contents and the URI
                // before falling back to "application/octet-stream"
                if (string.IsNullOrEmpty(contentType))
                {
                    string uriContentType;
                    if (TryToGetContentTypeFromUri(_httpRequest, out uriContentType))
                    {
                        contentType = uriContentType;
                    }
                }

                if (string.IsNullOrEmpty(contentType))
                {
                    contentType = DefaultContentType;
                }

                if (string.IsNullOrEmpty(contentType))
                {
                    contentType = DefaultContentType;
                }

                _effectiveContentType = contentType;
                return contentType;
            }
        }

        /// <summary>
        /// Gets a string array of client-supported MIME accept types. This version
        /// allows the accept types to be overridden according to the following
        /// rules:
        /// 1. If the query string contains a key called "format", its value is returned as the only accept type
        /// 2. Otherwise, if the request has an Accepts header, the list of content is returned
        /// 3. Otherwise, if the request has a content type, its value is returned
        /// </summary>
        public  string[] AcceptTypes
        {
            get
            {
                string[] acceptTypes = null;

                string contentType;
                if (TryToGetContentTypeFromUri(_httpRequest, out contentType))
                {
                    acceptTypes = new[] { contentType };
                }

                if (acceptTypes == null || acceptTypes.Length == 0)
                {
                    acceptTypes = _httpRequest.AcceptTypes;
                }

                if (acceptTypes == null || acceptTypes.Length == 0)
                {
                    acceptTypes = new[] { ContentType };
                }

                return acceptTypes;
            }
        }

        private static string GetHttpMethod(HttpRequestBase httpRequest)
        {
            string method = httpRequest.HttpMethod;
            if (string.Compare(method, "POST", StringComparison.OrdinalIgnoreCase) == 0)
            {
                string xHttpMethod = httpRequest.Headers[XHttpMethodOverrideHeader];
                if (!string.IsNullOrEmpty(xHttpMethod))
                {
                    return xHttpMethod.ToUpperInvariant();
                }

                ContentType requestFormat = GetRequestFormat(httpRequest);
                if (requestFormat != null && string.Compare(requestFormat.MediaType, UrlEncoded, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    string methodParameter = httpRequest.Form[HttpMethodOverrideFormId];
                    if (!string.IsNullOrEmpty(methodParameter))
                    {
                        return methodParameter.ToUpperInvariant();
                    }
                }

                string queryStringMethodParameter = httpRequest.QueryString[HttpMethodOverrideQueryStringId];
                if (!string.IsNullOrEmpty(queryStringMethodParameter))
                {
                    return queryStringMethodParameter.ToUpperInvariant();
                }
            }

            return method;
        }

        private static ContentType GetRequestFormat(HttpRequestBase request)
        {
            if (!string.IsNullOrEmpty(request.ContentType))
            {
                try
                {
                    return new ContentType(request.ContentType);
                }
                catch (FormatException)
                {
                    throw new HttpException((int)HttpStatusCode.UnsupportedMediaType, "Unsupported Media Type: '" + request.ContentType + "'");
                }
            }

            return new ContentType();
        }

        /// <summary>
        /// Gets the content type value from a URL parameter, if posssible.
        /// No validity checking is done on the value passed.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        private static bool TryToGetContentTypeFromUri(HttpRequestBase request, out string contentType)
        {
            string contentFormatFromQueryString = request.QueryString[ContentTypeQueryStringId];
            if (!string.IsNullOrEmpty(contentFormatFromQueryString))
            {
                contentType = Formats.FormatManager.GetContentTypeFromAlias(contentFormatFromQueryString);
                return true;
            }

            contentType = null;
            return false;
        }
    }
}
