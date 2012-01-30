using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NServiceMVC;
using AttributeRouting;

namespace NServiceMVC.Examples.HelloWorld.Controllers
{
    public class HelloController : ServiceController
    {
        [GET("hello")]
        public Models.HelloResponse Index()
        {
            return new Models.HelloResponse { GreetingType = "Hello" };
        }

        [GET("hello/{name}")]
        public Models.HelloResponse Name(string name)
        {
            return new Models.HelloResponse { GreetingType = "Hello", Name = name };
        }

    }
}
