using OpenManage.Client.Http;
using OpenManage.Client.Objects.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenManage.Client.Objects
{
    internal sealed class ObjectsClient : IObjectsClient
    {
        private readonly OpenManageHttpClient _httpClient;

        public ObjectsClient(OpenManageHttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public Task<ObjectResponse> CreateAsync(
            int objectType,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new CreateObjectRequest
            {
                ObjectType = objectType
            };

            return _httpClient.PostAsync<CreateObjectRequest, ObjectResponse>(
                OpenVaultEndpoint.Objects,
                request,
                cancellationToken);
        }

        public async Task DeleteAsync(
            long objectId,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            await _httpClient.DeleteAsync<bool>(
                    OpenVaultEndpoint.Object(objectId),
                    cancellationToken)
                .ConfigureAwait(false);
        }

        public Task<AttributeResponse> AddAttributeAsync(
            long objectId,
            int attributeId,
            string value,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var request = new AddAttributeRequest
            {
                AttributeId = attributeId,
                StringValue = value
            };

            return _httpClient.PostAsync<AddAttributeRequest, AttributeResponse>(
                OpenVaultEndpoint.ObjectAttributes(objectId),
                request,
                cancellationToken);
        }
    }
}
