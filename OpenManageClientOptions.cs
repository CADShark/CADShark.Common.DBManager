using System;

namespace OpenVault.Client
{
    /// <summary>
    /// Configuration for <see cref="OpenManageApi"/> / the future <c>OpenVaultClient</c> facade.
    /// The host application is responsible for supplying <see cref="BaseAddress"/>;
    /// the SDK does not ship a default OpenVault server address.
    /// </summary>
    public class OpenManageClientOptions
    {
        /// <summary>
        /// Base address of the OpenVault Web API (e.g. "https://openvault.internal:443/").
        /// Required — must be set by the host application before use.
        /// </summary>
        public Uri BaseAddress { get; set; }

        /// <summary>
        /// When true, TLS certificate validation errors are ignored for the underlying
        /// HttpClient. Intended only for local/dev environments with self-signed certs.
        /// Defaults to false: production usage must use a valid certificate or opt in explicitly.
        /// </summary>
        public bool IgnoreServerCertificateErrors { get; set; } = false;
    }
}