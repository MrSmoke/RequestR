namespace RequestR.Requests
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Responses;

    public class HttpRequest : HttpRequestBase<HttpResponse>
    {
        public HttpRequest(HttpMethod method, string resource) : base(method, resource)
        {
        }

        protected override Task<HttpResponse> ParseResponseAsync(HttpResponseMessage message, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponse(message));
        }
    }

    public class HttpRequest<T> : HttpRequestBase<HttpResponse<T>>
    {
        public HttpRequest(HttpMethod method, string resource) : base(method, resource)
        {
        }

        protected override async Task<HttpResponse<T>> ParseResponseAsync(HttpResponseMessage message, CancellationToken cancellationToken)
        {
            if (!message.IsSuccessStatusCode)
                return new HttpResponse<T>(message, default);

            var contentString = await message.Content.ReadAsStringAsync().ConfigureAwait(false);

            return new HttpResponse<T>(message, Deserialize(contentString));
        }

        protected T Deserialize(string input)
        {
            return Deserialize<T>(input);
        }
    }
}
