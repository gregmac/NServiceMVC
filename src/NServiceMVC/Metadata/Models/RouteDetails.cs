using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NServiceMVC.Metadata.Models
{
    public class RouteDetails
    {
        public class ParameterDetails
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public string Default { get; set; }
        }

        public RouteDetails()
        {
            Parameters = new List<ParameterDetails>();
        }

        public string Url { get; set; }
        public string Method { get; set; }
        public string Description { get; set; }
        public IEnumerable<ParameterDetails> Parameters { get; set; }

        public string ModelType { get; set; }
        /// <summary>
        /// If we have metadata details for this type (basically, is it a known model type?)
        /// </summary>
        public bool ModelHasMetadata { get; set; }
        public string ModelSampleJson { get; set; }
        public string ModelSampleXml { get; set; }

        /// <summary>
        /// Gets a "nice" url used for internal links
        /// </summary>
        public string NiceUrl
        {
            get
            {
                return Method.ToUpper() + "/" + Url.Replace("{", "").Replace("}", "");
            }
        }
    }
}
