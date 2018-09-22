namespace RequestR.Requests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;

    public interface IHttpRequest
    {
        HttpMethod Method { get; }
        IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers { get; }
        string Resource { get; }

        void AddHeader(string name, string value);
        void AddHeader(string name, IEnumerable<string> values);

        void AddParameter(string key, string value);
        void AddParameter(string key, string value, RequestParameterType parameterType);
        void AddParameter(string key, IEnumerable<string> values, RequestParameterType parameterType);

        void AddBody(object body);
        void AddBody(Stream stream);
        void AddBody(Stream stream, MediaTypeHeaderValue mediaType);
    }
}