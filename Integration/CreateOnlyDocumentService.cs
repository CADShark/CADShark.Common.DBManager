using OpenManage.Client.Files.Models;
using OpenManage.Client.Integration.Models;
using OpenManage.Client.Mapping;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace OpenManage.Client.Integration
{
    public sealed class CreateOnlyDocumentService : ICreateOnlyDocumentService
    {
        public const int SolidWorksPartObjectType = 1296;
        public const int SolidWorksAssemblyObjectType = 1361;
        public const int MainFileAttributeId = 1002;
        public const int MainFileLinkType = 4;

        private readonly OpenManageClient _client;
        private readonly IEngineeringPropertyMapper _mapper;

        public CreateOnlyDocumentService(
            OpenManageClient client,
            IEngineeringPropertyMapper mapper = null)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _mapper = mapper ?? new EngineeringPropertyMapper();
        }

        public async Task<CreateOnlyDocumentResult> CreateAsync(
            CreateOnlyDocumentRequest request,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (request.Document == null)
                throw new ArgumentException("Document is required.", nameof(request));
            if (request.PropertyMappings == null)
                throw new ArgumentException("Property mappings are required.", nameof(request));

            var result = new CreateOnlyDocumentResult();

            try
            {
                var workspace = new WindowsWorkspacePathService(request.WorkspaceRoot);
                var workspaceDocument = workspace.Resolve(request.Document.FullPath);
                var objectType = GetObjectType(request.Document.DocumentKind);
                var attributes = _mapper.Map(
                    request.Document.Properties,
                    request.PropertyMappings,
                    workspaceDocument.RelativePath);

                var createdObject = await _client.Objects
                    .CreateAsync(objectType, cancellationToken)
                    .ConfigureAwait(false);

                result.ObjectId = createdObject.ObjectId;
                result.VersionId = createdObject.VersionId;

                foreach (var attribute in attributes)
                {
                    await _client.Objects
                        .AddAttributeAsync(
                            createdObject.ObjectId,
                            attribute.AttributeId,
                            attribute.Value,
                            cancellationToken)
                        .ConfigureAwait(false);

                    result.AddedAttributeCount++;
                }

                var fileBody = File.ReadAllBytes(request.Document.FullPath);
                var file = await _client.Files
                    .AddAsync(
                        new AddFileRequest
                        {
                            FileName = workspaceDocument.FileName,
                            FileBody = fileBody,
                            ObjectLinkId = createdObject.ObjectId,
                            AttributeId = MainFileAttributeId,
                            LinkType = MainFileLinkType
                        },
                        cancellationToken)
                    .ConfigureAwait(false);

                result.FileId = file.FileId;
                result.IsSuccess = true;
                return result;
            }
            catch (Exception exception) when (!(exception is OperationCanceledException))
            {
                result.ErrorMessage = exception.Message;
                return result;
            }
        }

        private static int GetObjectType(EngineeringDocumentKind documentKind)
        {
            switch (documentKind)
            {
                case EngineeringDocumentKind.Part:
                    return SolidWorksPartObjectType;
                case EngineeringDocumentKind.Assembly:
                    return SolidWorksAssemblyObjectType;
                default:
                    throw new NotSupportedException(
                        "Only SOLIDWORKS parts and assemblies are supported by the first CreateOnly workflow.");
            }
        }
    }
}
