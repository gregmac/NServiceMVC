using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NServiceMVC.Examples.HelloWorld.Models
{
    public class ErrorSampleResponse
    {
        public string Message { get; set; }
        public string AdditionalInfo { get; set; }
        public int ResponseStatusId { get; set; }
    }
}