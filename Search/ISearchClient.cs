using OpenManage.Client.Search.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenManage.Client.Search
{
    public interface ISearchClient
    {
        Task<IReadOnlyList<long>> SearchAsync(
            SearchObjectsRequest request,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
