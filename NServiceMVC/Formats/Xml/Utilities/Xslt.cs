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
using System.IO;
using System.Xml;
using System.Text;
using System.Xml.Xsl;

namespace NServiceMVC.Formats.Xml.Utilities
{
    public static class Xslt
    {
        public static String Apply(String xml, XslCompiledTransform transform, Object extension, Encoding encoding, bool withBom)
        {
            var memStream = new MemoryStream();

            var xmlWriterSettings = new XmlWriterSettings();
            if (transform != null)
            {
                xmlWriterSettings = transform.OutputSettings.Clone();
            }

            if (encoding != null)
            {
                xmlWriterSettings.Encoding = encoding;
            }

            if (!string.IsNullOrEmpty(xml))
            {
                using (var stringReader = new StringReader(xml))
                {
                    var xmlReaderSettings = new XmlReaderSettings
                                                {
                                                    DtdProcessing = DtdProcessing.Parse,
                                                    XmlResolver = new XmlPreloadedResolver()
                                                };

                    using (XmlReader xReader = XmlReader.Create(stringReader, xmlReaderSettings))
                    {
                        XmlWriter xWriter = XmlWriter.Create(memStream, xmlWriterSettings);

                        // If a transform is present, use it, otherwise just copy the Xml into the memory stream
                        // applying the requested encoding as we go.
                        if (transform != null)
                        {
                            var args = new XsltArgumentList();
                            if (extension != null)
                            {
                                args.AddExtensionObject("ResourcesOverMvc:Extension", extension);
                            }

                            transform.Transform(xReader, args, xWriter);
                        }
                        else
                        {
				            xWriter.WriteNode(xReader, false);
				            xReader.Close();
                        }

                        xWriter.Flush();
                        xWriter.Close();
                    }
                }
            }

            // Optionally strip out any Byte Order Marking
            string result;
            if (withBom)
            {
                result = xmlWriterSettings.Encoding.GetString(memStream.ToArray());
            }
            else
            {
                memStream.Seek(0, SeekOrigin.Begin);
                TextReader textReader = new StreamReader(memStream);
                result = textReader.ReadToEnd();
            }

            return result;
        }
    }
}