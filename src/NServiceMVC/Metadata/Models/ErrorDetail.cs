using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NServiceMVC.Metadata.Models
{
    [DotLiquid.LiquidType("Message", "StackTrace", "InnerError")]
    public class ErrorDetail
    {
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public ErrorDetail InnerError { get; set; }
    }
}
