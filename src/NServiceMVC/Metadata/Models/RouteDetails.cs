using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NServiceMVC.Metadata.Models
{
    [DotLiquid.LiquidType("Url","Method","Description","Parameters","ModelType","ReturnType","NiceUrl")]
    public class RouteDetails
    {
        [DotLiquid.LiquidType("Name", "Type", "Default", "InUrl")]
        public class ParameterDetails
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public string Default { get; set; }
            public bool InUrl { get; set; }
        }

        public RouteDetails()
        {
            Parameters = new List<ParameterDetails>();
        }

        public string Url { get; set; }
        public string Method { get; set; }
        public string Description { get; set; }
        public IEnumerable<ParameterDetails> Parameters { get; set; }

        public ModelDetail ModelType { get; set; }
        
        public ModelDetail ReturnType { get; set; }

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
