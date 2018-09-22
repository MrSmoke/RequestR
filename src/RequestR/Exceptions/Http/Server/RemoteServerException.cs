namespace RequestR.Exceptions.Http.Server
{
    using System;
    using System.Net;

    public class RemoteServerException : HttpResponseException
    {
        public RemoteServerException(HttpStatusCode httpStatusCode, string message) : base(httpStatusCode, message)
        {
        }

        public RemoteServerException(HttpStatusCode httpStatusCode, string message, Exception innerException) : base(httpStatusCode, message, innerException)
        {
        }
    }
}