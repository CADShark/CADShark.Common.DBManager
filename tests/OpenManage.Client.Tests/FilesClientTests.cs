using OpenManage.Client.Files.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace OpenManage.Client.Tests
{
    public sealed class FilesClientTests
    {
        [Fact]
        public async Task AddAsync_UsesStorageRouteAndContract()
        {
            HttpRequestMessage captured = null;
            string body = null;
            var client = CreateClient(async request =>
            {
                captured = request;
                body = await request.Content.ReadAsStringAsync();
                return Envelope(
                    "{\"fileId\":25,\"fileName\":\"Part1.sldprt\"," +
                    "\"fileSize\":3,\"fileDate\":\"2026-08-13T12:00:00Z\"," +
                    "\"objectLinkId\":248,\"attributeId\":1002,\"linkType\":4}");
            });

            using (client)
            {
                var result = await client.Files.AddAsync(
                    new AddFileRequest
                    {
                        FileName = "Part1.sldprt",
                        FileBody = new byte[] { 1, 2, 3 },
                        ObjectLinkId = 248,
                        AttributeId = 1002,
                        LinkType = 4
                    });

                Assert.Equal(HttpMethod.Post, captured.Method);
                Assert.Equal("/api/storage", captured.RequestUri.PathAndQuery);
                Assert.Contains("\"FileName\":\"Part1.sldprt\"", body);
                Assert.Contains("\"FileBody\":\"AQID\"", body);
                Assert.Contains("\"ObjectLinkId\":248", body);
                Assert.Contains("\"AttributeId\":1002", body);
                Assert.Contains("\"LinkType\":4", body);
                Assert.Equal(25, result.FileId);
                Assert.Equal(248L, result.ObjectLinkId);
            }
        }

        [Fact]
        public async Task AddAsync_RejectsObjectIdOutsideCurrentServerRange()
        {
            using (var client = CreateClient(request => Envelope("null")))
            {
                await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
                    () => client.Files.AddAsync(
                        new AddFileRequest
                        {
                            FileName = "Part1.sldprt",
                            FileBody = new byte[] { 1 },
                            ObjectLinkId = (long)int.MaxValue + 1,
                            AttributeId = 1002,
                            LinkType = 4
                        }));
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
            return new OpenManageClient(
                new HttpClient(new StubHandler(handler))
                {
                    BaseAddress = new Uri("https://example.test/")
                });
        }

        private static HttpResponseMessage Envelope(string dataJson)
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    "{\"success\":true,\"data\":" + dataJson + ",\"error\":null}",
                    Encoding.UTF8,
                    "application/json")
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
                return _handler(request);
            }
        }
    }
}
