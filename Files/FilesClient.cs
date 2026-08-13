using OpenManage.Client.Files.Models;
using OpenManage.Client.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpenManage.Client.Files
{
    internal sealed class FilesClient : IFilesClient
    {
        private const int CurrentServerMaximumObjectLinkId = int.MaxValue;
        private readonly OpenManageHttpClient _httpClient;

        public FilesClient(OpenManageHttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public Task<FileResponse> AddAsync(
            AddFileRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(request.FileName))
                throw new ArgumentException("File name cannot be empty.", nameof(request));
            if (request.FileBody == null || request.FileBody.Length == 0)
                throw new ArgumentException("File body cannot be empty.", nameof(request));
            if (request.ObjectLinkId <= 0)
                throw new ArgumentException("Object link ID must be positive.", nameof(request));
            if (request.ObjectLinkId > CurrentServerMaximumObjectLinkId)
                throw new ArgumentOutOfRangeException(
                    nameof(request),
                    "The current OpenVault Storage API accepts ObjectLinkId only within the Int32 range.");
            if (request.AttributeId <= 0)
                throw new ArgumentException("Attribute ID must be positive.", nameof(request));
            if (request.LinkType <= 0)
                throw new ArgumentException("Link type must be positive.", nameof(request));

            return _httpClient.PostAsync<AddFileRequest, FileResponse>(
                OpenVaultEndpoint.Storage,
                request,
                cancellationToken);
        }
    }
}
