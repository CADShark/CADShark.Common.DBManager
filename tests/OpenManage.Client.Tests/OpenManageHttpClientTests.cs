using OpenManage.Client.Http;
using OpenManage.Client.Objects.Models;
using OpenManage.Client.Search.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace OpenManage.Client.Tests
{
    public sealed class OpenManageHttpClientTests
    {
        [Fact]
        public async Task PostAsync_Returns_Data_From_Successful_Envelope()
        {
            var transport = CreateTransport(
                HttpStatusCode.OK,
                "{\"success\":true,\"data\":{\"objectId\":5000000000,\"versionId\":7,\"objectType\":1296},\"error\":null}");

            var result = await transport.PostAsync<object, ObjectResponse>(
                "api/objects",
                new { objectType = 1296 });

            Assert.Equal(5000000000L, result.ObjectId);
            Assert.Equal(7, result.VersionId);
            Assert.Equal(1296, result.ObjectType);
        }

        [Fact]
        public async Task PostAsync_Throws_Typed_Exception_For_Business_Error()
        {
            var transport = CreateTransport(
                HttpStatusCode.OK,
                "{\"success\":false,\"data\":null,\"error\":{\"code\":\"duplicate\",\"message\":\"Already exists\"}}");

            var exception = await Assert.ThrowsAsync<OpenManageApiException>(
                () => transport.PostAsync<object, ObjectResponse>(
                    "api/objects",
                    new { objectType = 1 }));

            Assert.Equal(OpenManageErrorKind.Api, exception.ErrorKind);
            Assert.Equal("duplicate", exception.ErrorCode);
            Assert.Equal("Already exists", exception.Message);
        }

        [Theory]
        [InlineData(HttpStatusCode.NotFound)]
        [InlineData(HttpStatusCode.Conflict)]
        [InlineData(HttpStatusCode.InternalServerError)]
        public async Task GetAsync_Throws_Typed_Exception_For_Http_Error(
            HttpStatusCode statusCode)
        {
            var transport = CreateTransport(
                statusCode,
                "{\"success\":false,\"data\":null,\"error\":{\"code\":\"http_error\",\"message\":\"Request failed\"}}");

            var exception = await Assert.ThrowsAsync<OpenManageApiException>(
                () => transport.GetAsync<ObjectResponse>("api/objects/1"));

            Assert.Equal(OpenManageErrorKind.Http, exception.ErrorKind);
            Assert.Equal(statusCode, exception.StatusCode);
            Assert.Equal("http_error", exception.ErrorCode);
        }

        [Fact]
        public async Task GetAsync_Throws_Diagnostic_Exception_For_Invalid_Json()
        {
            var transport = CreateTransport(HttpStatusCode.OK, "not-json");

            var exception = await Assert.ThrowsAsync<OpenManageApiException>(
                () => transport.GetAsync<ObjectResponse>("api/objects/1"));

            Assert.Equal(OpenManageErrorKind.InvalidResponse, exception.ErrorKind);
            Assert.Equal("not-json", exception.ResponseBody);
        }

        [Fact]
        public async Task GetAsync_Throws_Network_Exception_When_Backend_Is_Unavailable()
        {
            var httpClient = new HttpClient(
                new ThrowingHandler(new HttpRequestException("offline")))
            {
                BaseAddress = new Uri("https://example.test/")
            };
            var transport = new OpenManageHttpClient(
                httpClient,
                new JsonHttpContentSerializer());

            var exception = await Assert.ThrowsAsync<OpenManageApiException>(
                () => transport.GetAsync<ObjectResponse>("api/objects/1"));

            Assert.Equal(OpenManageErrorKind.Network, exception.ErrorKind);
            Assert.IsType<HttpRequestException>(exception.InnerException);
        }

        [Fact]
        public async Task GetAsync_Propagates_Requested_Cancellation()
        {
            var transport = CreateTransport(
                HttpStatusCode.OK,
                "{\"success\":true,\"data\":{},\"error\":null}");
            var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => transport.GetAsync<ObjectResponse>(
                    "api/objects/1",
                    cancellation.Token));
        }

        [Fact]
        public async Task Search_Request_Uses_Public_ObjectTypeIds_And_Server_Json_Name()
        {
            string requestBody = null;
            var handler = new StubHandler(async request =>
            {
                requestBody = await request.Content.ReadAsStringAsync();
                return Response(
                    HttpStatusCode.OK,
                    "{\"success\":true,\"data\":{\"objectIds\":[5000000000]},\"error\":null}");
            });

            using (var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://example.test/")
            })
            using (var client = new OpenManageClient(httpClient))
            {
                var result = await client.Search.SearchAsync(
                    new SearchObjectsRequest
                    {
                        VersionId = 3,
                        Filters = new List<AttributeFilter>
                        {
                            new AttributeFilter
                            {
                                AttributeId = 2001,
                                Value = "A-001",
                                ObjectTypeIds = new List<int> { 1296 }
                            }
                        }
                    });

                Assert.Single(result);
                Assert.Equal(5000000000L, result[0]);
                Assert.Contains("\"ObjectTypeID\":[1296]", requestBody);
                Assert.Contains("\"versionId\":3", requestBody);
            }
        }

        private static OpenManageHttpClient CreateTransport(
            HttpStatusCode statusCode,
            string responseBody)
        {
            var httpClient = new HttpClient(
                new StubHandler(_ => Task.FromResult(Response(statusCode, responseBody))))
            {
                BaseAddress = new Uri("https://example.test/")
            };

            return new OpenManageHttpClient(
                httpClient,
                new JsonHttpContentSerializer());
        }

        private static HttpResponseMessage Response(
            HttpStatusCode statusCode,
            string body)
        {
            return new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };
        }

        private sealed class StubHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _handler;

            public StubHandler(
                Func<HttpRequestMessage, Task<HttpResponseMessage>> handler)
            {
                _handler = handler;
            }

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();
                return _handler(request);
            }
        }

        private sealed class ThrowingHandler : HttpMessageHandler
        {
            private readonly Exception _exception;

            public ThrowingHandler(Exception exception)
            {
                _exception = exception;
            }

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                throw _exception;
            }
        }
    }
}
