namespace RequestR.Requests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Exceptions;
    using Helpers;
    using Responses;
    using Serialization;

    public abstract class HttpRequestBase<TResponse> : IHttpRequest where TResponse : HttpResponse
    {
        public HttpMethod Method { get; }
        public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Headers => _headers;
        public string Resource { get; }

        public ISerializer Serializer { set; private get; }

        private HttpContent _content;
        private readonly Dictionary<string, List<RequestParameterValue>> _parameters = new Dictionary<string, List<RequestParameterValue>>();
        private readonly HttpRequestHeaders _headers = new HttpRequestHeaders();

        protected HttpRequestBase(HttpMethod method, string resource)
        {
            Method = method;
            Resource = resource;
        }

        /// <summary>
        /// Set to true if we should throw an exception on 404
        /// </summary>
        protected bool ThrowOnNotFound { get; set; } = false;

        /// <summary>
        /// Returns true of the method can have a body
        /// </summary>
        protected bool CanMethodHaveBody => Method == HttpMethod.Post || Method == HttpMethod.Put;

        public void AddHeader(string name, string value)
        {
            _headers.Add(name, value);
        }

        public void AddHeader(string name, IEnumerable<string> values)
        {
            _headers.Add(name, values);
        }

        public void AddParameter(string key, string value)
        {
            AddParameter(key, value, RequestParameterType.QueryOrBody);
        }

        public void AddParameter(string key, string value, RequestParameterType parameterType)
        {
            AddParameter(key, new RequestParameterValue(value, parameterType));
        }

        public void AddParameter(string key, IEnumerable<string> values, RequestParameterType parameterType)
        {
            ValidateParameterType(parameterType);

            var parameterValues = values.Select(v => new RequestParameterValue(v, parameterType));

            GetParameterValues(key).AddRange(parameterValues);
        }

        public void AddBody(object body)
        {
            if (!CanMethodHaveBody)
                throw new InvalidOperationException($"Cannot add body to a {Method} request");

            //Check if we have a serializer
            if (Serializer == null)
                throw new InvalidOperationException("Cannot serialize body. No serializer defined");

            var mediaType = "application/" + Serializer.Format;

            string content;
            try
            {
                content = Serializer.Serialize(body);
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to serialize request body", ex);
            }

            _content = new StringContent(content, Encoding.UTF8, mediaType);
        }

        public void AddBody(Stream stream)
        {
            AddBody(stream, new MediaTypeHeaderValue("application/octet-stream"));
        }

        public void AddBody(Stream stream, MediaTypeHeaderValue mediaType)
        {
            _content = new StreamContent(stream)
            {
                Headers =
                {
                    ContentType = mediaType
                }
            };
        }

        protected T Deserialize<T>(string input)
        {
            try
            {
                return Serializer.Deserialize<T>(input);
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to deserialize value", ex);
            }
        }

        protected string Serialize(object obj)
        {
            try
            {
                return Serializer.Serialize(obj);
            }
            catch (Exception ex)
            {
                throw new SerializationException("Failed to serialize value", ex);
            }
        }

        protected internal virtual Task OnRequestExecuting(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected internal virtual Task OnRequestExecuted(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        protected abstract Task<TResponse> ParseResponseAsync(HttpResponseMessage message, Error error, CancellationToken cancellationToken);

        protected virtual bool TryParseErrorBody(string content, out ErrorBody error)
        {
            error = null;
            return false;
        }

        protected virtual ErrorBody GetDefaultErrorBody(HttpStatusCode httpStatusCode, string defaultMessage)
        {
            return new ErrorBody
            {
                Message = ErrorHelper.GetDefaultErrorMessage(httpStatusCode) ?? defaultMessage
            };
        }

        protected virtual async Task<Error> GetErrorAsync(HttpResponseMessage message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));

            //There is no error
            if (message.IsSuccessStatusCode)
                return null;

            if (message.Content != null)
            {
                var content = await message.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (TryParseErrorBody(content, out var errorBody))
                    return new Error(message.StatusCode, errorBody);
            }

            return new Error(message.StatusCode, GetDefaultErrorBody(message.StatusCode, message.ReasonPhrase));
        }

        protected virtual void HandleError(Error error)
        {
            if (error.HttpStatusCode == HttpStatusCode.NotFound && !ThrowOnNotFound)
                return;

            throw ErrorHelper.GetErrorException(error);
        }

        internal async Task<TResponse> GetResponseAsync(HttpResponseMessage message, CancellationToken cancellationToken)
        {
            // Check if the response is an error
            var error = await GetErrorAsync(message).ConfigureAwait(false);

            // if we have an error, handle it
            if (error != null)
                HandleError(error);

            // if the error wasn't handled, parse the response
            var response = await ParseResponseAsync(message, error, cancellationToken).ConfigureAwait(false);

            // add our error if we had one
            if (error != null)
                response.Error = error;

            return response;
        }

        internal HttpContent GetContent()
        {
            if (_content != null)
                return _content;

            var bodyParams = GetParameters(RequestParameterType.Body).ToList();

            if (bodyParams.Count > 0)
                return new FormUrlEncodedContent(bodyParams);

            return null;
        }

        internal IEnumerable<KeyValuePair<string, string>> GetParameters(RequestParameterType parameterType)
        {
            foreach (var p in _parameters)
            {
                foreach (var value in p.Value)
                {
                    if ((value.Type & parameterType) != 0)
                        yield return new KeyValuePair<string, string>(p.Key, value.Value);
                }
            }
        }

        private void ValidateParameterType(RequestParameterType type)
        {
            var isBodyOnly = (type & RequestParameterType.Query) == 0;

            if (!isBodyOnly)
                return;

            if (!CanMethodHaveBody)
                throw new ArgumentException($"{Method} cannot have body parameters");

            if (_content != null)
                throw new ArgumentException("Cannot add body parameter. Request already has a body");
        }

        private void AddParameter(string key, RequestParameterValue parameterValue)
        {
            ValidateParameterType(parameterValue.Type);

            GetParameterValues(key).Add(parameterValue);
        }

        private List<RequestParameterValue> GetParameterValues(string key)
        {
            if (!_parameters.TryGetValue(key, out var paramList))
            {
                paramList = new List<RequestParameterValue>();
                _parameters.Add(key, paramList);
            }

            return paramList;
        }
    }
}