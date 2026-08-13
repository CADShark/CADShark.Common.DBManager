using OpenManage.Client.Files.Models;
using System.Threading;
using System.Threading.Tasks;

namespace OpenManage.Client.Files
{
    public interface IFilesClient
    {
        Task<FileResponse> AddAsync(
            AddFileRequest request,
            CancellationToken cancellationToken = default(CancellationToken));
    }
}
