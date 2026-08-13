using OpenManage.Client.Objects.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OpenManage.Client.Objects
{
    public interface IObjectsClient
    {
        Task<ObjectResponse> CreateAsync(
            int objectType,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<ObjectResponse> GetByIdAsync(
            long objectId,
            CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(
            long objectId,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<AttributeResponse> AddAttributeAsync(
            long objectId,
            int attributeId,
            string value,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<AttributeResponse> GetAttributeByIdAsync(
            long objectId,
            int attributeId,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<AttributeResponse> GetAttributeByNameAsync(
            long objectId,
            string attributeName,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<AttributeResponse> UpdateAttributeAsync(
            long objectId,
            int attributeId,
            string value,
            CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAttributeAsync(
            long objectId,
            int attributeId,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IReadOnlyList<ObjectTypeHierarchyRecord>> GetHierarchyAsync(
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IReadOnlyList<ObjectNavigatorRecord>> GetNavigatorRecordsAsync(
            int objectType,
            CancellationToken cancellationToken = default(CancellationToken));

        Task<IReadOnlyList<ObjectCompositionRecord>> GetCompositionAsync(
            long objectId,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
