namespace RequestR.HttpClient
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Requests;
    using Responses;

    public static class HttpClientExtensions
    {
        public static Task<TResponse> ExecuteAsync<TResponse>(this HttpClient client, HttpRequestBase<TResponse> httpRequest)
            where TResponse : HttpResponse
        {
            return ExecuteAsync(client, httpRequest, CancellationToken.None);
        }

        public static async Task<TResponse> ExecuteAsync<TResponse>(this HttpClient http,
            HttpRequestBase<TResponse> httpRequest, CancellationToken cancellationToken)
            where TResponse : HttpResponse
        {
            if (http == null)
                throw new ArgumentNullException(nameof(http));

            if (httpRequest == null)
                throw new ArgumentNullException(nameof(httpRequest));

            using (var request = new HttpRequestMessage(httpRequest.Method, httpRequest.Resource))
            {
                await httpRequest.OnRequestExecuting(cancellationToken).ConfigureAwait(false);

                // add all our request headers
                foreach (var header in httpRequest.Headers)
                    request.Headers.Add(header.Key, header.Value);

                var content = httpRequest.GetContent();

                try
                {
                    request.Content = content;

                    using (var response = await http.SendAsync(request, cancellationToken).ConfigureAwait(false))
                    {
                        await httpRequest.OnRequestExecuted(cancellationToken).ConfigureAwait(false);

                        return await httpRequest.GetResponseAsync(response, cancellationToken).ConfigureAwait(false);
                    }
                }
                finally
                {
                    // dispose our content if we have any
                    content?.Dispose();
                }
            }
        }
    }
}
