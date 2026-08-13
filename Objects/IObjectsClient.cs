using OpenManage.Client.Objects.Models;
using System.Threading;
using System.Threading.Tasks;

namespace OpenManage.Client.Objects
{
    public interface IObjectsClient
    {
        Task<ObjectResponse> CreateAsync(
            int objectType,
            CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(
            long objectId,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<AttributeResponse> AddAttributeAsync(
            long objectId,
            int attributeId,
            string value,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
