using System;

namespace OpenVault.Client
{
public class OpenVaultApiOptions
    {
        public Uri BaseAddress { get; set; } = new Uri("https://192.168.1.109:443/");
        public bool IgnoreServerCertificateErrors { get; set; } = true;
    }
}
