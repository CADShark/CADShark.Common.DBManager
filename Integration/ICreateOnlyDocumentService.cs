using OpenManage.Client.Integration.Models;
using System.Threading;
using System.Threading.Tasks;

namespace OpenManage.Client.Integration
{
    public interface ICreateOnlyDocumentService
    {
        Task<CreateOnlyDocumentResult> CreateAsync(
            CreateOnlyDocumentRequest request,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
