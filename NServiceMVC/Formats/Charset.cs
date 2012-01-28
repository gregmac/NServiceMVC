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

namespace NServiceMVC.Formats
{
    public struct Charset : IComparable<Charset>
    {
        private static readonly char[] Delimiters = { ';', '=' };
        private const float DefaultWeight = 1;
        private Encoding _encoding;
        private float _weight;
        private int _ordinal;

        public Charset(string value) : this(value, 0) { }
        public Charset(string value, int ordinal)
        {
            _encoding = null;
            _weight = 0;
            _ordinal = ordinal;

            ParseInternal(ref this, value);
        }

        public Charset(Encoding encoding, float weight)
        {
            _encoding = encoding;
            _weight = weight;
            _ordinal = 0;
        }

        public Encoding Encoding { get {return _encoding;}}
        public float Weight { get {return _weight;}}
        public bool IsEmpty { get {return _encoding == null;}}

        public static Charset Parse(string value)
        {
            var item = new Charset();
            ParseInternal(ref item, value);
            return item;
        }

        public static Charset Parse(string value, int ordinal)
        {
            Charset item = Parse(value);
            item._ordinal = ordinal;
            return item;
        }

        private static void ParseInternal(ref Charset target, string value)
        {
            string[] parts = value.Split(Delimiters, 3);
            if (parts.Length > 0)
            {
                try
                {
                    target._encoding = Encoding.GetEncoding(parts[0].Trim());
                }
                catch (ArgumentException)
                {
                    // Ignore unsupported encodings 
                    target._encoding = null;
                }

                target._weight = DefaultWeight;
            }

            if (parts.Length == 3)
            {
                float.TryParse(parts[2], out target._weight);
            }
        }

        public int CompareTo(Charset other)
        {
            int value = _weight.CompareTo(other._weight);
            if (value == 0)
            {
                int ord = -_ordinal;
                value = ord.CompareTo(-other._ordinal);
            }
            return value;
        }

        public static int CompareByWeightAsc(Charset x, Charset y)
        {
            return x.CompareTo(y);
        }

        public static int CompareByWeightDesc(Charset x, Charset y)
        {
            return -x.CompareTo(y);
        }
    }
}