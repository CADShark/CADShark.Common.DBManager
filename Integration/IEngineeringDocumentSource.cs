using System.Threading;
using System.Threading.Tasks;
using OpenManage.Client.Integration.Models;

namespace OpenManage.Client.Integration
{
    public interface IEngineeringDocumentSource
    {
        Task<EngineeringDocumentInfo> GetActiveDocumentAsync(
            CancellationToken cancellationToken);
    }
}
