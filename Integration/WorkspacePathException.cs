using System;

namespace OpenManage.Client.Integration
{
    public sealed class WorkspacePathException : Exception
    {
        public WorkspacePathException(string message)
            : base(message)
        {
        }
    }
}
