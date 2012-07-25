using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting;
using AttributeRouting.Web.Mvc;

namespace NServiceMVC.Examples.HelloWorld.Controllers
{
    public class ArraySampleController : ServiceController
    {
        //
        // GET: /ArraySample/

        [GET("arraysample/string/list")]
        public List<string> StringList()
        {
            return new List<string>() { "one", "two", "three" };
        }

        [GET("arraysample/string/enumerable")]
        public IEnumerable<string> StringEnumerable()
        {
            return new List<string>() { "one", "two", "three" };
        }

        [GET("arraysample/string/array")]
        public string[] StringArray()
        {
            return new string[] { "one", "two", "three" };
        }

        [GET("arraysample/hello/{name}")]
        public List<Models.HelloResponse> HelloList(string name)
        {
            return new List<Models.HelloResponse>() { 
                new Models.HelloResponse() { GreetingType = "Hello", Name = name },
                new Models.HelloResponse() { GreetingType = "Bonjour", Name = name },
                new Models.HelloResponse() { GreetingType = "¡Hola", Name = name },
                new Models.HelloResponse() { GreetingType = "こんにちは", Name = name },
                new Models.HelloResponse() { GreetingType = "שלום", Name = name },
                new Models.HelloResponse() { GreetingType = "привет", Name = name },
                new Models.HelloResponse() { GreetingType = "Hallå", Name = name },
            };
        }
    }
}
