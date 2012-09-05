using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NServiceMVC.Examples.HelloWorld.Models
{
    public class ComplexModel1
    {
        public DateTime StartDate { get; set; }
        public Nullable<DateTime> EndDate { get; set; }
        public Nullable<Int64> OptionalInt { get; set; }
        public SimpleModel NestedSimpleModel { get; set; }
    }
}