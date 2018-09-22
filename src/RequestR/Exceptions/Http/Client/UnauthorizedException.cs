namespace RequestR.Exceptions.Http.Client
{
    using System;
    using System.Net;

    public class UnauthorizedException : ClientHttpResponseException
    {
        public UnauthorizedException(HttpStatusCode httpStatusCode, string message) : base(httpStatusCode, message)
        {
        }

        public UnauthorizedException(HttpStatusCode httpStatusCode, string message, Exception innerException) : base(httpStatusCode, message, innerException)
        {
        }
    }
}