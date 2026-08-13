using OpenManage.Client.Integration.Models;

namespace OpenManage.Client.Integration
{
    public interface IWorkspacePathService
    {
        string WorkspaceRoot { get; }

        WorkspaceDocumentInfo Resolve(string fullPath);
    }
}
