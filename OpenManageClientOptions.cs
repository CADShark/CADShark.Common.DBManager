using System;

namespace OpenManage.Client
{
    public sealed class OpenManageClientOptions
    {
        public Uri BaseAddress { get; set; }

        public bool IgnoreServerCertificateErrors { get; set; }
    }
}
