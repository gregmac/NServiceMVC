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
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace NServiceMVC.Formats.Xml.Utilities
{
    public class Xml
    {
        public static string ObjectToXml(object toBeSerialised, bool includeXmlDeclaration)
        {
            string result = "";

            if (toBeSerialised != null)
            {
                // Make the output string pretty
                var xmlStringBuilder = new StringBuilder();
                var xmlWriterSettings = new XmlWriterSettings
                                            {
                                                OmitXmlDeclaration = !includeXmlDeclaration,
                                                Indent = true,
                                                NewLineOnAttributes = true
                                            };
                XmlWriter xmlWriter = XmlWriter.Create(xmlStringBuilder, xmlWriterSettings);

                // Try to find the root attribute so we can set the default namespace to use
                var objectType = toBeSerialised.GetType();
                var defaultNamespace = string.Empty;
                var xmlRootAttributes = toBeSerialised.GetType().GetCustomAttributes(typeof(XmlRootAttribute), true) as XmlRootAttribute[];
                if (xmlRootAttributes != null && xmlRootAttributes.Length > 0)
                {
                    defaultNamespace = xmlRootAttributes[0].Namespace;
                }

                // Use this object to prevent the serilaizer from adding extra "Ambient" namespaces
                var xmlSerializerNamespaces = new XmlSerializerNamespaces();
                xmlSerializerNamespaces.Add(String.Empty, defaultNamespace);

                // Using this constructor should mean the serializer is cached... beware of using any other
                // constructor it may cause memory leaks!!!!
                var xmlSerializer = new XmlSerializer(objectType);

                xmlSerializer.Serialize(xmlWriter, toBeSerialised, xmlSerializerNamespaces);
                xmlWriter.Flush();
                xmlWriter.Close();
                result = xmlStringBuilder.ToString();
            }

            return result;
        }

        public static T XmlToObject<T>(string xml) where T : class
        {
            // Using this constructor should mean the serializer is cached... beware of using any other
            // constructor it may cause memory leaks!!!!
            var xmlSerializer = new XmlSerializer(typeof(T));
            return xmlSerializer.Deserialize(new StringReader(xml)) as T;
        }
    }
}