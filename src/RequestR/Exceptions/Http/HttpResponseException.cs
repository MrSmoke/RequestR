namespace RequestR.Exceptions.Http
{
    using System;
    using System.Net;

    /// <summary>
    /// The exception thrown when there is a http error
    /// </summary>
    public class HttpResponseException :  Exception
    {
        public HttpStatusCode HttpStatusCode { get; }

        public HttpResponseException(HttpStatusCode httpStatusCode, string message) : base(message)
        {
            HttpStatusCode = httpStatusCode;
        }

        public HttpResponseException(HttpStatusCode httpStatusCode, string message, Exception innerException) : base(message, innerException)
        {
            HttpStatusCode = httpStatusCode;
        }
    }
}
