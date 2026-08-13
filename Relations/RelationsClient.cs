using OpenManage.Client.Http;
using OpenManage.Client.Relations.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenManage.Client.Relations
{
    internal sealed class RelationsClient : IRelationsClient
    {
        private readonly OpenManageHttpClient _httpClient;

        public RelationsClient(OpenManageHttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public Task<ObjectRelationResponse> CreateAsync(
            long parentObjectId,
            long childObjectId,
            int relationType,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new CreateObjectRelationRequest
            {
                ParentObjectId = parentObjectId,
                ChildObjectId = childObjectId,
                RelationType = relationType
            };

            return _httpClient.PostAsync<CreateObjectRelationRequest, ObjectRelationResponse>(
                OpenVaultEndpoint.ObjectRelations,
                request,
                cancellationToken);
        }

        public Task<ObjectRelationResponse> MoveAsync(
            long relationId,
            long newParentObjectId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new MoveObjectRelationRequest
            {
                NewParentObjectId = newParentObjectId
            };

            return _httpClient.PostAsync<MoveObjectRelationRequest, ObjectRelationResponse>(
                OpenVaultEndpoint.ObjectRelationMove(relationId),
                request,
                cancellationToken);
        }

        public async Task DeleteAsync(
            long relationId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await _httpClient.DeleteAsync<bool>(
                    OpenVaultEndpoint.ObjectRelation(relationId),
                    cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
