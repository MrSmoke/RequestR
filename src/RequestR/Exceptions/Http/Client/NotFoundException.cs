namespace RequestR.Exceptions.Http.Client
{
    using System;
    using System.Net;

    public class NotFoundException : ClientHttpResponseException
    {
        public NotFoundException(HttpStatusCode httpStatusCode, string message) : base(httpStatusCode, message)
        {
        }

        public NotFoundException(HttpStatusCode httpStatusCode, string message, Exception innerException) : base(httpStatusCode, message, innerException)
        {
        }
    }
}