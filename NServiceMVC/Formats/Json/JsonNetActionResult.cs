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
using System.Web.Mvc;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace NServiceMVC.Formats.Json
{
    public class JsonNetActionResult : ActionResult
    {
        public object Data { get; set; }
        public CharsetList AcceptCharsetList { get; set; }
        public JsonSerializerSettings JsonSerializerSettings { get; set; }
        public Formatting Formatting { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            // Select the encoding to use
            Encoding encoding = Encoding.Default;
            if (AcceptCharsetList != null && AcceptCharsetList.Count > 0)
            {
                encoding = AcceptCharsetList[0].Encoding;
            }

            string dataAsJson;
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream, encoding))
            using (var jsonWriter = new JsonTextWriter(streamWriter))
            {
                jsonWriter.Formatting = Formatting;

                Newtonsoft.Json.JsonSerializer jsonSerializer = Newtonsoft.Json.JsonSerializer.Create(JsonSerializerSettings);
                jsonSerializer.Serialize(jsonWriter, Data);

                jsonWriter.Flush();
                jsonWriter.Close();

                dataAsJson = encoding.GetString(memoryStream.ToArray());
            }

            context.HttpContext.Response.ContentEncoding = encoding;
            context.HttpContext.Response.Charset = encoding.WebName;
            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.Write(dataAsJson);
        }
    }
}