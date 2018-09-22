namespace RequestR.Exceptions.Http.Server
{
    using System;
    using System.Net;

    public class NetworkException : RemoteServerException
    {
        public NetworkException(HttpStatusCode httpStatusCode, string message) : base(httpStatusCode, message)
        {
        }

        public NetworkException(HttpStatusCode httpStatusCode, string message, Exception innerException) : base(httpStatusCode, message, innerException)
        {
        }
    }
}