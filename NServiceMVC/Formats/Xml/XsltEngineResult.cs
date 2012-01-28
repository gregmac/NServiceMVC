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
using System.Xml.Xsl;

namespace NServiceMVC.Formats.Xml
{
    /// <summary>
    /// Holds the result of a seacrh for a transform... if the transform is found
    /// then the compiled version is eld in the result. If it isn't found, a list
    /// of the directories that were searched is held.
    /// </summary>
    public class XsltEngineResult
    {
        public IEnumerable<string> SearchedLocations { get; private set; }
        public XslCompiledTransform XslCompiledTransform { get; private set; }

        public XsltEngineResult(IEnumerable<string> searchedLocations)
        {
            if (searchedLocations == null)
            {
                throw new ArgumentNullException("searchedLocations");
            }

            SearchedLocations = searchedLocations;
        }

        public XsltEngineResult(XslCompiledTransform xslCompiledTransform)
        {
            if (xslCompiledTransform == null)
            {
                throw new ArgumentNullException("xslCompiledTransform");
            }

            XslCompiledTransform = xslCompiledTransform;
        }
    }
}