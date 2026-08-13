using OpenManage.Client.Relations.Models;
using System.Threading;
using System.Threading.Tasks;

namespace OpenManage.Client.Relations
{
    public interface IRelationsClient
    {
        Task<ObjectRelationResponse> CreateAsync(
            long parentObjectId,
            long childObjectId,
            int relationType,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<ObjectRelationResponse> MoveAsync(
            long relationId,
            long newParentObjectId,
            CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(
            long relationId,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
