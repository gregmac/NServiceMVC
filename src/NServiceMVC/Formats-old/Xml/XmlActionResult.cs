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
using System.Text;
using System.Web.Mvc;

namespace NServiceMVC.Formats.Xml
{
    public class XmlActionResult : ActionResult
    {
        public bool IgnoreMissingXslt { get; set; }
        public string XsltName { get; set; }
        public object Data { get; set; }
        public CharsetList AcceptCharsetList { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (string.IsNullOrEmpty(XsltName))
            {
                XsltName = context.RouteData.GetRequiredString("action");
            }

            // Select the encoding to use
            Encoding encoding = Encoding.Default;
            if (AcceptCharsetList != null && AcceptCharsetList.Count > 0)
            {
                encoding = AcceptCharsetList[0].Encoding;
            }

            var xsltSerializer = new XsltSerializer();
            string dataAsExternalXml = xsltSerializer.Serialize(Data, XsltName, context, AcceptCharsetList, encoding, IgnoreMissingXslt);

            context.HttpContext.Response.ContentEncoding = encoding;
            context.HttpContext.Response.Charset = encoding.WebName;
            context.HttpContext.Response.ContentType = "application/xml";
            context.HttpContext.Response.Write(dataAsExternalXml);
        }
    }
}