using OpenManage.Client.Http;
using OpenManage.Client.Objects;
using OpenManage.Client.Relations;
using OpenManage.Client.Search;
using System;
using System.Net.Http;

namespace OpenManage.Client
{
    public sealed class OpenManageClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly bool _ownsHttpClient;

        public OpenManageClient(OpenManageClientOptions options)
            : this(CreateHttpClient(options), true)
        {
        }

        public OpenManageClient(HttpClient httpClient)
            : this(httpClient, false)
        {
        }

        private OpenManageClient(HttpClient httpClient, bool ownsHttpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _ownsHttpClient = ownsHttpClient;

            var transport = new OpenManageHttpClient(
                _httpClient,
                new JsonHttpContentSerializer());

            Objects = new ObjectsClient(transport);
            Relations = new RelationsClient(transport);
            Search = new SearchClient(transport);
        }

        public IObjectsClient Objects { get; }

        public IRelationsClient Relations { get; }

        public ISearchClient Search { get; }

        public void Dispose()
        {
            if (_ownsHttpClient)
                _httpClient.Dispose();
        }

        private static HttpClient CreateHttpClient(OpenManageClientOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (options.BaseAddress == null)
            {
                throw new ArgumentException(
                    "OpenManageClientOptions.BaseAddress must be set by the host application.",
                    nameof(options));
            }

            var handler = new HttpClientHandler();

            if (options.IgnoreServerCertificateErrors)
            {
                handler.ServerCertificateCustomValidationCallback =
                    (message, certificate, chain, errors) => true;
            }

            return new HttpClient(handler)
            {
                BaseAddress = options.BaseAddress
            };
        }
    }
}
