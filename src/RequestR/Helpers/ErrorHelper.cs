namespace RequestR.Helpers
{
    using System;
    using System.Net;
    using Exceptions.Http;
    using Exceptions.Http.Client;
    using Exceptions.Http.Server;

    public static class ErrorHelper
    {
        public static string GetDefaultErrorMessage(HttpStatusCode httpStatusCode)
        {
            switch (httpStatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    return "Authorization has been denied for this request";
                case HttpStatusCode.Forbidden:
                    return "You are not allowed to access this resource";

                default:
                    return null;
            }
        }

        public static HttpResponseException GetErrorException(Error error)
        {
            if (error == null)
                throw new ArgumentNullException(nameof(error));

            if (error.Body == null)
                throw new ArgumentNullException(nameof(error.Body));

            var message = error.Body.Message;
            var statusCode = error.HttpStatusCode;
            var intStatusCode = (int) statusCode;

            // ignore all success/informational status codes
            if (intStatusCode <= 299)
                return null;

            // ignore all redirect error codes too. The HttpClient should handle these
            if (intStatusCode >= 300 && intStatusCode <= 399)
                return null;

            // client errors
            if (intStatusCode >= 400 && intStatusCode <= 499)
            {
                switch (statusCode)
                {
                    // bad data errors
                    case HttpStatusCode.BadRequest:
                        return new BadRequestException(statusCode, message);
                    case HttpStatusCode.NotFound:
                        return new NotFoundException(statusCode, message);

                    // authentication errors
                    case HttpStatusCode.Forbidden:
                    case HttpStatusCode.Unauthorized:
                        return new UnauthorizedException(statusCode, message);

                    // bad request errors
                    case HttpStatusCode.Gone:
                    case HttpStatusCode.MethodNotAllowed:
                    case HttpStatusCode.NotAcceptable:
                    case HttpStatusCode.RequestEntityTooLarge:
                    case HttpStatusCode.RequestUriTooLong:
                    case HttpStatusCode.UnsupportedMediaType:
                        return new BadRequestException(statusCode, message);
                }

                throw new ClientHttpResponseException(statusCode, message);
            }

            // server errors
            if (intStatusCode >= 500 && intStatusCode <= 599)
            {
                switch (error.HttpStatusCode)
                {
                    // network errors
                    case HttpStatusCode.BadGateway:
                    case HttpStatusCode.GatewayTimeout:
                        return new NetworkException(statusCode, message);

                    // server errors
                    default:
                        return new RemoteServerException(statusCode, message);
                }
            }

            // everything else
            throw new HttpResponseException(statusCode, message);
        }
    }
}
