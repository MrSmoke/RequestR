namespace RequestR.Extensions
{
    using System.Collections.Generic;
    using Requests;

    public static class HttpRequestExtensions
    {
        public static void AddQueryParameter(this IHttpRequest request, string key, string value)
        {
            request.AddParameter(key, value, RequestParameterType.Query);
        }

        public static void AddQueryParameter(this IHttpRequest request, string key, IEnumerable<string> values)
        {
            request.AddParameter(key, values, RequestParameterType.Query);
        }
    }
}
