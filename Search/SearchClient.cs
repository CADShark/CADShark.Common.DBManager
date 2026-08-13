using OpenManage.Client.Http;
using OpenManage.Client.Search.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenManage.Client.Search
{
    internal sealed class SearchClient : ISearchClient
    {
        private readonly OpenManageHttpClient _httpClient;

        public SearchClient(OpenManageHttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<IReadOnlyList<long>> SearchAsync(
            SearchObjectsRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var response = await _httpClient
                .PostAsync<SearchObjectsRequest, SearchObjectsResponse>(
                    OpenVaultEndpoint.ObjectSearch,
                    request,
                    cancellationToken)
                .ConfigureAwait(false);

            return response.ObjectIds ?? new List<long>();
        }
    }
}
