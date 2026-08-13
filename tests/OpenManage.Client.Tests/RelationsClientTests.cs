using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace OpenManage.Client.Tests
{
    public sealed class RelationsClientTests
    {
        [Fact]
        public async Task CreateAsync_Uses_Relations_Route_And_Contract()
        {
            HttpRequestMessage captured = null;
            string requestBody = null;
            var client = CreateClient(async request =>
            {
                captured = request;
                requestBody = await request.Content.ReadAsStringAsync();
                return Envelope(
                    "{\"relationId\":7000000000,\"parentObjectId\":5000000000," +
                    "\"childLogicalId\":6000000000,\"resolvedChildObjectId\":6000000001," +
                    "\"resolvedChildVersionId\":2,\"relationType\":1014," +
                    "\"createDate\":\"2026-08-13T12:00:00Z\"}");
            });

            using (client)
            {
                var result = await client.Relations.CreateAsync(
                    5000000000L,
                    6000000001L,
                    1014);

                Assert.Equal(HttpMethod.Post, captured.Method);
                Assert.Equal("/api/object-relations", captured.RequestUri.PathAndQuery);
                Assert.Contains("\"parentObjectId\":5000000000", requestBody);
                Assert.Contains("\"childObjectId\":6000000001", requestBody);
                Assert.Contains("\"relationType\":1014", requestBody);
                Assert.Equal(7000000000L, result.RelationId);
                Assert.Equal(6000000000L, result.ChildLogicalId);
                Assert.Equal(6000000001L, result.ResolvedChildObjectId);
                Assert.Equal(2, result.ResolvedChildVersionId);
            }
        }

        [Fact]
        public async Task MoveAsync_Uses_Relation_Move_Route()
        {
            HttpRequestMessage captured = null;
            string requestBody = null;
            var client = CreateClient(async request =>
            {
                captured = request;
                requestBody = await request.Content.ReadAsStringAsync();
                return Envelope(
                    "{\"relationId\":7,\"parentObjectId\":99,\"childLogicalId\":8," +
                    "\"resolvedChildObjectId\":8,\"resolvedChildVersionId\":1," +
                    "\"relationType\":1014,\"createDate\":\"2026-08-13T12:00:00Z\"}");
            });

            using (client)
            {
                await client.Relations.MoveAsync(7, 99);

                Assert.Equal(HttpMethod.Post, captured.Method);
                Assert.Equal("/api/object-relations/7/move", captured.RequestUri.PathAndQuery);
                Assert.Contains("\"newParentObjectId\":99", requestBody);
            }
        }

        [Fact]
        public async Task DeleteAsync_Uses_Relation_Route()
        {
            HttpRequestMessage captured = null;
            var client = CreateClient(request =>
            {
                captured = request;
                return Envelope("true");
            });

            using (client)
            {
                await client.Relations.DeleteAsync(7000000000L);

                Assert.Equal(HttpMethod.Delete, captured.Method);
                Assert.Equal("/api/object-relations/7000000000", captured.RequestUri.PathAndQuery);
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
