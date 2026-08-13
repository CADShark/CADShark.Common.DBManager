using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace OpenManage.Client.Tests
{
    public sealed class ObjectsClientDetailsTests
    {
        [Fact]
        public async Task GetByIdAsync_Uses_Long_Object_Route_And_Deserializes_Object()
        {
            HttpRequestMessage captured = null;
            var client = CreateClient(request =>
            {
                captured = request;
                return Envelope("{\"objectId\":5000000000,\"versionId\":3,\"objectType\":1296}");
            });

            using (client)
            {
                var result = await client.Objects.GetByIdAsync(5000000000L);

                Assert.Equal(HttpMethod.Get, captured.Method);
                Assert.Equal("/api/objects/5000000000", captured.RequestUri.PathAndQuery);
                Assert.Equal(5000000000L, result.ObjectId);
                Assert.Equal(3, result.VersionId);
                Assert.Equal(1296, result.ObjectType);
            }
        }

        [Fact]
        public async Task GetAttributeByIdAsync_Uses_Attribute_Route()
        {
            HttpRequestMessage captured = null;
            var client = CreateClient(request =>
            {
                captured = request;
                return Envelope("{\"attributeId\":2001,\"stringValue\":\"A-001\"}");
            });

            using (client)
            {
                var result = await client.Objects.GetAttributeByIdAsync(5000000000L, 2001);

                Assert.Equal(HttpMethod.Get, captured.Method);
                Assert.Equal("/api/objects/5000000000/attributes/2001", captured.RequestUri.PathAndQuery);
                Assert.Equal(2001, result.AttributeId);
                Assert.Equal("A-001", result.StringValue);
            }
        }

        [Fact]
        public async Task GetAttributeByNameAsync_Encodes_Attribute_Name()
        {
            HttpRequestMessage captured = null;
            var client = CreateClient(request =>
            {
                captured = request;
                return Envelope("{\"attributeId\":2001,\"stringValue\":\"A-001\"}");
            });

            using (client)
            {
                await client.Objects.GetAttributeByNameAsync(42, "Part name/number");

                Assert.Equal(HttpMethod.Get, captured.Method);
                Assert.Equal(
                    "/api/objects/42/attributes/by-name/Part%20name%2Fnumber",
                    captured.RequestUri.PathAndQuery);
            }
        }

        [Fact]
        public async Task UpdateAttributeAsync_Uses_Put_And_Server_Json_Contract()
        {
            HttpRequestMessage captured = null;
            string requestBody = null;
            var client = CreateClient(async request =>
            {
                captured = request;
                requestBody = await request.Content.ReadAsStringAsync();
                return Envelope("{\"attributeId\":2001,\"stringValue\":\"A-002\"}");
            });

            using (client)
            {
                var result = await client.Objects.UpdateAttributeAsync(
                    5000000000L,
                    2001,
                    "A-002");

                Assert.Equal(HttpMethod.Put, captured.Method);
                Assert.Equal("/api/objects/5000000000/attributes", captured.RequestUri.PathAndQuery);
                Assert.Contains("\"attributeId\":2001", requestBody);
                Assert.Contains("\"stringValue\":\"A-002\"", requestBody);
                Assert.Equal("A-002", result.StringValue);
            }
        }

        [Fact]
        public async Task DeleteAttributeAsync_Uses_Attribute_Route()
        {
            HttpRequestMessage captured = null;
            var client = CreateClient(request =>
            {
                captured = request;
                return Envelope("true");
            });

            using (client)
            {
                await client.Objects.DeleteAttributeAsync(5000000000L, 2001);

                Assert.Equal(HttpMethod.Delete, captured.Method);
                Assert.Equal("/api/objects/5000000000/attributes/2001", captured.RequestUri.PathAndQuery);
            }
        }

        private static OpenManageClient CreateClient(
            Func<HttpRequestMessage, HttpResponseMessage> handler)
        {
            return CreateClient(request => Task.FromResult(handler(request)));
        }

        private static OpenManageClient CreateClient(
            Func<HttpRequestMessage, Task<HttpResponseMessage>> handler)
        {
            var httpClient = new HttpClient(new StubHandler(handler))
            {
                BaseAddress = new Uri("https://example.test/")
            };

            return new OpenManageClient(httpClient);
        }

        private static HttpResponseMessage Envelope(string dataJson)
        {
            var json = "{\"success\":true,\"data\":" + dataJson + ",\"error\":null}";
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
        }

        private sealed class StubHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _handler;

            public StubHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> handler)
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
    }
}
