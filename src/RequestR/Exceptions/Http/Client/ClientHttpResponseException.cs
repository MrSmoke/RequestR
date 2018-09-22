namespace RequestR.Exceptions.Http.Client
{
    using System;
    using System.Net;

    /// <summary>
    /// The exception thrown for all 4xx HTTP status codes
    /// </summary>
    public class ClientHttpResponseException : HttpResponseException
    {
        public ClientHttpResponseException(HttpStatusCode httpStatusCode, string message) : base(httpStatusCode, message)
        {
        }

        public ClientHttpResponseException(HttpStatusCode httpStatusCode, string message, Exception innerException) : base(httpStatusCode, message, innerException)
        {
        }
    }
}