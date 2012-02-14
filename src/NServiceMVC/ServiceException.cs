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
        /// <summary>
        /// Throw a 500 - Internal Server Error, with the specified model as the content. 
        /// The model will be serialized according to the accept-types, just like a normal
        /// service response.
        /// </summary>
        public ServiceException(object model) 
            : this(System.Net.HttpStatusCode.InternalServerError, model) { }

        /// <summary>
        /// Return a specific HTTP status code with the specified model as the content. 
        /// The model will be serialized according to the accept-types, just like a normal
        /// service response.
        /// </summary>
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
