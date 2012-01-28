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

using System.Net.Mime;
using System.Web.Mvc;

namespace NServiceMVC.Formats
{
    public interface IRequestFormatHandler
    {
        /// <summary>
        /// Return true if the specified friendly name can be mapped to a
        /// content type (e.g. translate 'xml' into 'application/xml'). If the mapping
        /// can be performed return the content type that the friendlyName maps to
        /// </summary>
        bool TryToMapFormatFriendlyName(string friendlyName, out string contentType);

        /// <summary>
        /// Return true if the request content can be desrialized
        /// </summary>
        bool TryDeserializeRequestRepresentation(ControllerContext controllerContext, ModelBindingContext bindingContext, ContentType requestContentType, out object model);
    }
}
