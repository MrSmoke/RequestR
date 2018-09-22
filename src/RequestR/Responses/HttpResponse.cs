namespace RequestR.Responses
{
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;

    public class HttpResponse
    {
        public HttpStatusCode HttpStatusCode { get; }
        public HttpResponseHeaders Headers { get; }

        public Error Error { get; internal set; }

        public HttpResponse(HttpResponseMessage message)
        {
            HttpStatusCode = message.StatusCode;
            Headers = message.Headers;
        }
    }

    public class HttpResponse<T> : HttpResponse
    {
        public T Data { get; internal set; }

        public HttpResponse(HttpResponseMessage message, T data) : base(message)
        {
            Data = data;
        }
    }
}
