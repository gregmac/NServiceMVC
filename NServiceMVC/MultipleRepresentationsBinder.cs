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

using System.Net;
using System.Web.Mvc;
using System.Web;
using System;

namespace NServiceMVC
{
    class MultipleRepresentationsBinder : IModelBinder
    {
        readonly IModelBinder _inner;

        public MultipleRepresentationsBinder() : this(ModelBinders.Binders.DefaultBinder) { }
        public MultipleRepresentationsBinder(IModelBinder inner)
        {
            _inner = inner;
        }

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            // Only bind values that have not been extracted from the URI if the request actually has a body to deserialize
            if (!controllerContext.RouteData.Values.ContainsKey(bindingContext.ModelName) && RequestHasBody(controllerContext.HttpContext.Request))
            {
                object model = null;
                //TryBindModel(controllerContext, bindingContext, out model);
                return model;
            }

            return _inner.BindModel(controllerContext, bindingContext);
        }

        //private void TryBindModel(ControllerContext controllerContext, ModelBindingContext bindingContext, out object model)
        //{
        //    if (!FormatManager.Current.TryDeserializeRequestRepresentation(controllerContext, bindingContext, out model))
        //    {
        //        // If the data was not handled by any of the format handlers, try the default binder
        //        model = _inner.BindModel(controllerContext, bindingContext);
        //    }
        //}

        private static bool RequestHasBody(HttpRequestBase request)
        {
            return request.ContentLength > 0 || string.Compare("chunked", request.Headers["Transfer-Encoding"], StringComparison.OrdinalIgnoreCase) == 0;
        }
    }
}
