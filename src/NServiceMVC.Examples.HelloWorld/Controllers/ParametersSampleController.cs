using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting;
using AttributeRouting.Web.Mvc;

namespace NServiceMVC.Examples.HelloWorld.Controllers
{
    public class ParametersSampleController : ServiceController
    {
        [GET("params/one")]
        public string One(string A, string B, string C)
        {
            return String.Format("A = {0}, B = {1}, C = {2}", A,B,C);
        }

        [GET("params/two/{inUrl}")]
        public string Two(string inUrl, string param1, string param2)
        {
            return String.Format("inUrl = {0}, param1 = {1}, param2 = {2}", inUrl, param1, param2);
        }

        //[GET("params/three/{inUrl}")]
        [POST("params/three/{inUrl}")]
        public string Three(string inUrl, Models.SimpleModel mod, string A, string D)
        {
            return String.Format("inUrl = {0}, mod.A = {1}, mod.B = {2}, mod.C = {3}, A = {4}, D = {5}", inUrl, mod.A, mod.B, mod.C, A, D);
        }

    }
}
