using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using NServiceMVC;
using AttributeRouting;
using System.ComponentModel;

namespace NServiceMVC.Examples.HelloWorld.Controllers
{
    public class HelloController : ServiceController
    {
        [GET("hello")]
        public Models.HelloResponse Index()
        {
            return new Models.HelloResponse { GreetingType = "Hello" };
        }

        [POST("hello")]
        public Models.HelloResponse Index(Models.NameDetails details)
        {
            return new Models.HelloResponse { 
                GreetingType = "Hello", 
                Name = details.FirstName + ' ' + details.LastName,
            };
        }

        [GET("hello/{name}")]
        [Description("Says hello to the name passed in the URL")]
        public Models.HelloResponse Name(string name)
        {
            return new Models.HelloResponse { GreetingType = "Hello", Name = name };
        }

    }
}
