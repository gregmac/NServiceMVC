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
using System.Xml.Xsl;
using System.Xml.Serialization;
using System.IO;
using NServiceMVC.Formats.Xml.Utilities;

namespace NServiceMVC.Formats.Xml
{
    /// <summary>
    /// 
    /// </summary>
    public class XsltSerializer
    {
        private VirtualPathProviderXsltEngine _xsltEngine;

        /// <summary>
        /// For now hardcoded to always return an engine that looks in the virtual path
        /// </summary>
        internal VirtualPathProviderXsltEngine XsltEngine
        {
            get { return _xsltEngine ?? (_xsltEngine = new VirtualPathProviderXsltEngine()); }
        }

        /// <summary>
        /// Turn the data into an xml string, then transform
        /// </summary>
        public string Serialize(object objectToSerialize, string transformFileName, ControllerContext controllerContext, CharsetList acceptCharsetList, Encoding encoding, bool ignoreMissingXslt)
        {
            string serializedObject = Utilities.Xml.ObjectToXml(objectToSerialize, false);

            XslCompiledTransform xslCompiledTransform = FindTransform(controllerContext, transformFileName, ignoreMissingXslt);

            // External representations do need Byte Order Mark
            //return Xslt.Apply(serializedObject, xslCompiledTransform, new XsltExtension(), encoding, true);
            return Xslt.Apply(serializedObject, xslCompiledTransform, null, encoding, true);
        }

        /// <summary>
        /// Transform the string then deserialize into the expected type
        /// </summary>
        internal object Deserialize(string serializedObject, ControllerContext controllerContext, string transformFileName, Type targetType, bool ignoreMissingXslt)
        {
            XslCompiledTransform xslCompiledTransform = FindTransform(controllerContext, transformFileName, ignoreMissingXslt);

            // Internal representations don't need Byte Order Mark
            //string representationAsInternalXml = Xslt.Apply(serializedObject, xslCompiledTransform, new XsltExtension(), null, false);
            string representationAsInternalXml = Xslt.Apply(serializedObject, xslCompiledTransform, null, null, false);

            // Using this constructor should mean the serializer is cached... beware of using any other
            // constructor it may cause memory leaks!!!!
            var xmlSerializer = new XmlSerializer(targetType);
            return xmlSerializer.Deserialize(new StringReader(representationAsInternalXml));
        }


        /// <summary>
        /// Try to find the required transform or throw an exception listing the locations tried.
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="transformFileName"></param>
        /// <param name="ignoreMissingXslt"></param>
        /// <returns></returns>
        private XslCompiledTransform FindTransform(ControllerContext controllerContext, string transformFileName, bool ignoreMissingXslt)
        {
            // Try to find the transform
            XsltEngineResult xsltEngineResult = XsltEngine.FindTransform(controllerContext, transformFileName);

            if (xsltEngineResult.XslCompiledTransform == null && !ignoreMissingXslt)
            {
                var builder = new StringBuilder();
                foreach (string str in xsltEngineResult.SearchedLocations)
                {
                    builder.AppendLine();
                    builder.Append(str);
                }

                throw new InvalidOperationException(string.Format("The Xslt '{0}' could not be found. The following locations were searched:{1}", new object[] { transformFileName, builder }));
            }

            return xsltEngineResult.XslCompiledTransform;
        }
    }
}