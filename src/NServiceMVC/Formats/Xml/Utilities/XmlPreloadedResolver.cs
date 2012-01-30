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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Xml;

namespace NServiceMVC.Formats.Xml.Utilities
{
    [Flags]
    public enum XmlKnownDtds
    {
        All = 0xffff,
        None = 0,
        Xhtml10 = 1,
        Xhtml11 = 2
    }

    public class XmlPreloadedResolver : XmlResolver
    {
        private readonly Dictionary<Uri, XmlKnownDtdData> _mappings;

        private static readonly XmlKnownDtdData[] Xhtml11Dtd = new[] 
        {                        
            new XmlKnownDtdData("-//W3C//DTD XHTML 1.1//EN", "http://www.w3.org/TR/xhtml1/DTD/xhtml1.dtd", "xhtml11-flat.dtd")
        };

        public XmlPreloadedResolver()
        {
            _mappings = new Dictionary<Uri, XmlKnownDtdData>(2);
            AddKnownDtd(Xhtml11Dtd);
        }

        private void AddKnownDtd(IEnumerable<XmlKnownDtdData> dtdSet)
        {
            foreach (var data in dtdSet)
            {
                _mappings.Add(new Uri(data.PublicId, UriKind.RelativeOrAbsolute), data);
                _mappings.Add(new Uri(data.SystemId, UriKind.RelativeOrAbsolute), data);
            }
        }

        public override object GetEntity(Uri absoluteUri, string role, Type ofObjectToReturn)
        {
            XmlKnownDtdData data;
            if (absoluteUri == null)
            {
                throw new ArgumentNullException("absoluteUri");
            }
            if (!_mappings.TryGetValue(absoluteUri, out data))
            {
                throw new XmlException("CannotResolveUrl");
            }
            if ((ofObjectToReturn == null) || (ofObjectToReturn == typeof(Stream)))
            {
                return data.AsStream();
            }

            throw new XmlException("Unsupported Class");
        }

        public override Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            if ((relativeUri != null) && relativeUri.StartsWith("-//W3C//"))
            {
                if (Xhtml11Dtd.Any(t => relativeUri == t.PublicId))
                {
                    return new Uri(relativeUri, UriKind.Relative);
                }
            }
            return base.ResolveUri(baseUri, relativeUri);
        }

        public override ICredentials Credentials
        {
            set { throw new NotImplementedException(); }
        }

        private class XmlKnownDtdData
        {
            // Fields
            internal readonly string PublicId;
            private readonly string _resourceName;
            internal readonly string SystemId;

            // Methods
            internal XmlKnownDtdData(string publicId, string systemId, string resourceName)
            {
                PublicId = publicId;
                SystemId = systemId;
                _resourceName = resourceName;
            }

            internal Stream AsStream()
            {
                return Assembly.GetExecutingAssembly().GetManifestResourceStream("ResourcesOverMvc.Web.Mvc.Utilities." + _resourceName);
            }
        }
    }
}