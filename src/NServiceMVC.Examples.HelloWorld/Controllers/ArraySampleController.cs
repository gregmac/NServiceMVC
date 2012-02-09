using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting;

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

    }
}
