using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using NServiceMVC.Examples.HelloWorld.Models;

namespace NServiceMVC.Examples.HelloWorld.Controllers
{
    public class TypeSamplesController : ServiceController
    {
        //
        // GET: /TypeSamples/

        [GET("typesamples/string")]
        public string StringSample()
        {
            return "This is a string";
        }

        [GET("typesamples/bool")]
        public bool BoolSample()
        {
            return true;
        }

        [GET("typesamples/int")]
        public int IntSample()
        {
            return 42;
        }

        [GET("typesamples/nullable/bool")]
        public Nullable<Boolean> NullableBoolTrue()
        {
            return true;
        }


        [GET("typesamples/nullable/bool/null")]
        public Nullable<Boolean> NullableBoolNull()
        {
            return null;
        }

        [GET("typesamples/nullable/DateTime")]
        public Nullable<DateTime> NullableDateTime()
        {
            return DateTime.Now;
        }


        [GET("typesamples/complex")]
        public Models.ComplexModel1 Complex()
        {
            return new ComplexModel1()
                       {
                           EndDate = DateTime.Now,
                           StartDate = DateTime.Now.AddDays(-2),
                           NestedSimpleModel = new SimpleModel()
                                                   {
                                                       A = "A value",
                                                       C = "test"
                                                   }
                       };
        }

    }
}
