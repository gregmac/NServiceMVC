using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NServiceMVC.Metadata.Models
{
    public class MetadataSummary
    {
        public IEnumerable<RouteDetails> Routes { get; set; }
        public IEnumerable<ModelDetail> Models { get; set; }
    }
}