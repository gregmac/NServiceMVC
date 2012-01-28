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

namespace NServiceMVC.Formats
{
    /// <summary>
    /// Provides a collection for working with Accept-Charset http headers 
    /// </summary>
    /// <remarks>
    /// accept-encoding spec: 
    ///		http://www.w3.org/Protocols/rfc2616/rfc2616-sec14.html
    /// </remarks>
    public sealed class CharsetList : List<Charset>
    {
        private static readonly char[] Delimiters = { ',' };

        public CharsetList(string values) : this(string.IsNullOrEmpty(values) ? new string[0] : values.Split(Delimiters, StringSplitOptions.RemoveEmptyEntries)){ }
        public CharsetList(IEnumerable<string> values)
        {
            int ordinal = -1;
            foreach (string value in values)
            {
                // Ignore wild cards because if the q is greater than 0 we are allowed to use
                // any charset we want. If the q is 0 then RFC 2616 allows us to supply
                // anything we want anyway!
                string trimmedValue = value.Trim();
                if (!string.IsNullOrEmpty(trimmedValue) && trimmedValue[0] != '*')
                {
                    Charset charset = Charset.Parse(trimmedValue, ++ordinal);
                    if (!charset.IsEmpty)
                    {
                        Add(charset);
                    }
                }
            }

            DefaultSort();
        }

        public new void Add(Charset item)
        {
            base.Add(item);
        }

        public void DefaultSort()
        {
            Sort(Charset.CompareByWeightDesc);
        }
    }
}
