using System;
using System.Collections.Generic;

namespace OpenManage.Client.Integration.Models
{
    public sealed class EngineeringDocumentInfo
    {
        public EngineeringDocumentInfo()
        {
            Properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public string FullPath { get; set; }

        public string Configuration { get; set; }

        public EngineeringDocumentKind DocumentKind { get; set; }

        public IDictionary<string, string> Properties { get; }
    }
}
