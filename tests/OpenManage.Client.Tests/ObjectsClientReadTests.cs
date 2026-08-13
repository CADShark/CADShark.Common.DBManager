using OpenManage.Client.Objects.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace OpenManage.Client.Tests
{
    public sealed class ObjectsClientReadTests
    {
        [Fact]
        public async Task GetHierarchyAsync_Uses_Expected_Route_And_Deserializes_Icons()
        {
            string requestUri = null;
            var client = CreateClient(request =>
            {
                requestUri = request.RequestUri.PathAndQuery;
                return Envelope(
                    "[{\"objectType\":10,\"parentTypeId\":0,\"objectTypeName\":\"Documents\",\"icon\":\"AQI=\",\"iconPlaceholder32x16\":\"AwQ=\",\"iconPlaceholder64x32\":\"BQY=\"}]");
            });

            using (client)
            {
                var records = await client.Objects.GetHierarchyAsync();

                var record = Assert.Single(records);
                Assert.Equal("/api/objects/hierarchy", requestUri);
                Assert.Equal(10, record.ObjectType);
                Assert.Equal("Documents", record.ObjectTypeName);
                Assert.Equal(new byte[] { 1, 2 }, record.Icon);
                Assert.Equal(new byte[] { 3, 4 }, record.IconPlaceholder32x16);
                Assert.Equal(new byte[] { 5, 6 }, record.IconPlaceholder64x32);
            }
        }

        [Fact]
        public async Task GetNavigatorRecordsAsync_Uses_ObjectType_Route()
        {
            string requestUri = null;
            var client = CreateClient(request =>
            {
                requestUri = request.RequestUri.PathAndQuery;
                return Envelope(
                    "[{\"objectId\":5000000000,\"objectType\":1296,\"versionId\":4,\"iconPlaceholder32x16\":\"AQ==\",\"designation\":\"A-001\",\"name\":\"Part\"}]");
            });

            using (client)
            {
                var records = await client.Objects.GetNavigatorRecordsAsync(1296);

                var record = Assert.Single(records);
                Assert.Equal("/api/objects/navigator/1296", requestUri);
                Assert.Equal(5000000000L, record.ObjectId);
                Assert.Equal(4, record.VersionId);
                Assert.Equal("A-001", record.Designation);
                Assert.Equal("Part", record.Name);
            }
        }

        [Fact]
        public async Task GetCompositionAsync_Uses_Long_ObjectId_And_Deserializes_Relations()
        {
            string requestUri = null;
            var client = CreateClient(request =>
            {
                requestUri = request.RequestUri.PathAndQuery;
                return Envelope(
                    "[{\"relationId\":7000000000,\"parentObjectId\":5000000000,\"objectId\":6000000000,\"objectType\":1296,\"versionId\":2,\"icon\":null,\"iconPlaceholder32x16\":null,\"designation\":\"A-002\",\"name\":\"Child\"}]");
            });

            using (client)
            {
                var records = await client.Objects.GetCompositionAsync(5000000000L);

                var record = Assert.Single(records);
                Assert.Equal("/api/objects/5000000000/composition", requestUri);
                Assert.Equal(7000000000L, record.RelationId);
                Assert.Equal(5000000000L, record.ParentObjectId);
                Assert.Equal(6000000000L, record.ObjectId);
                Assert.Equal("Child", record.Name);
            }
        }

        private static OpenManageClient CreateClient(
            Func<HttpRequestMessage, HttpResponseMessage> handler)
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
            private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;

            public StubHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
            {
                _handler = handler;
            }

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();
                return Task.FromResult(_handler(request));
            }
        }
    }
}
