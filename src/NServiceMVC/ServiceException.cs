using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NServiceMVC
{
    public interface IServiceException
    {
        System.Net.HttpStatusCode StatusCode { get; }
        object Model { get; }
    }

    /// <summary>
    /// An exception that causes the REST service to return a specific http status code and 
    /// </summary>
    public class ServiceException : Exception, IServiceException 
    {
        public ServiceException(object model) 
            : this(System.Net.HttpStatusCode.OK, model) { }

        public ServiceException(System.Net.HttpStatusCode statusCode, object model) 
            : base(model.ToString()) 
        {
            StatusCode = statusCode;
            Model = model;
        }

        public System.Net.HttpStatusCode StatusCode { get; set; }
        
        public object Model { get; set; }
    }
}
