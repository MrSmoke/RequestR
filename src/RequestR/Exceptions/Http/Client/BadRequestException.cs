namespace RequestR.Exceptions.Http.Client
{
    using System;
    using System.Net;

    public class BadRequestException : ClientHttpResponseException
    {
        public BadRequestException(HttpStatusCode httpStatusCode, string message) : base(httpStatusCode, message)
        {
        }

        public BadRequestException(HttpStatusCode httpStatusCode, string message, Exception innerException) : base(httpStatusCode, message, innerException)
        {
        }
    }
}