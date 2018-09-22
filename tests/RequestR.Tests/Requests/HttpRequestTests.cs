namespace RequestR.Tests.Requests
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using RequestR.Requests;
    using Xunit;

    public class HttpRequestTests
    {
        [Fact]
        public void AddParameter_GetParameters_Query()
        {
            var request = new HttpRequest(HttpMethod.Get, "v1/sweet");

            request.AddParameter("query1", "hello1");
            request.AddParameter("query2", "hello2");

            var queryParams = request.GetParameters(RequestParameterType.Query).ToList();

            Assert.Equal(2, queryParams.Count);

            Assert.Equal("query1", queryParams[0].Key);
            Assert.Equal("query2", queryParams[1].Key);

            Assert.Equal("hello1", queryParams[0].Value);
            Assert.Equal("hello2", queryParams[1].Value);
        }

        [Fact]
        public void AddParameter_GetParameters_Query_DuplicateKeys_ReturnsAll()
        {
            var request = new HttpRequest(HttpMethod.Get, "v1/sweet");

            request.AddParameter("query1", "hello1");
            request.AddParameter("query1", "hello2");
            request.AddParameter("query1", "hello3");

            var queryParams = request.GetParameters(RequestParameterType.Query).ToList();

            Assert.Equal(3, queryParams.Count);

            Assert.Equal("query1", queryParams[0].Key);
            Assert.Equal("query1", queryParams[1].Key);
            Assert.Equal("query1", queryParams[2].Key);


            Assert.Equal("hello1", queryParams[0].Value);
            Assert.Equal("hello2", queryParams[1].Value);
            Assert.Equal("hello3", queryParams[2].Value);
        }

        [Fact]
        public void AddParameter_GetParameters_Body()
        {
            var request = new HttpRequest(HttpMethod.Get, "v1/sweet");

            request.AddParameter("query1", "hello1");
            request.AddParameter("query2", "hello2");

            var queryParams = request.GetParameters(RequestParameterType.Body).ToList();

            Assert.Equal(2, queryParams.Count);
        }

        [Fact]
        public void AddParameter_GetParameters_QueryOrBody()
        {
            var request = new HttpRequest(HttpMethod.Get, "v1/sweet");

            request.AddParameter("query1", "hello1");
            request.AddParameter("query2", "hello2");

            var queryParams = request.GetParameters(RequestParameterType.QueryOrBody).ToList();

            Assert.Equal(2, queryParams.Count);
        }

        [Fact]
        public void AddParameter_Body_GetParameters_Query_ReturnsNone()
        {
            var request = new HttpRequest(HttpMethod.Post, "v1/sweet");

            request.AddParameter("query1", "hello1", RequestParameterType.Body);
            request.AddParameter("query2", "hello2", RequestParameterType.Body);

            var queryParams = request.GetParameters(RequestParameterType.Query);

            Assert.Empty(queryParams);
        }

        [Fact]
        public void AddParameter_Body_HttpMethodGet_ThrowsArgumentException()
        {
            var request = new HttpRequest(HttpMethod.Get, "v1/sweet");

            Assert.Throws<ArgumentException>(() => request.AddParameter("query1", "hello1", RequestParameterType.Body));
        }
    }
}
