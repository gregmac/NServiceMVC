namespace NServiceMVC
{
    public interface IServiceController
    {
        /// <summary>
        /// Information about the request. 
        /// </summary>
        WebStack.HttpRequestInfo RequestInfo { get; set; }
    }
}
