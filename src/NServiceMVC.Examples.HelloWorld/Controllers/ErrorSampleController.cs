using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttributeRouting;
using NServiceMVC;

namespace NServiceMVC.Examples.HelloWorld.Controllers
{
    public class ErrorSampleController : ServiceController
    {
        //
        // GET: /ErrorSample/

        public ActionResult Index()
        {
            return View();
        }

        [GET("errorsample/exception")]
        public string Exception()
        {
            throw new Exception("This is a regular System.Exception, with no inner exception");
        }

        [GET("errorsample/404")]
        public string NotFound()
        {
            throw new ServiceException(System.Net.HttpStatusCode.NotFound, new Models.ErrorSampleResponse { Message = "This is a 404 not found.", AdditionalInfo = "The model return can be any data" } );
        }

        [GET("errorsample/503")]
        public string ServiceUnavail()
        {
            throw new ServiceException(System.Net.HttpStatusCode.ServiceUnavailable, new Models.ErrorSampleResponse { Message = "503 Unavailable", AdditionalInfo = "The model return can be any data", ResponseStatusId = 123456 });
        }

    }
}
