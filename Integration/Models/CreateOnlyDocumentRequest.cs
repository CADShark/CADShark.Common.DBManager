using OpenManage.Client.Mapping;
using System.Collections.Generic;

namespace OpenManage.Client.Integration.Models
{
    public sealed class CreateOnlyDocumentRequest
    {
        public EngineeringDocumentInfo Document { get; set; }
        public string WorkspaceRoot { get; set; }
        public IEnumerable<PropertyAttributeMapping> PropertyMappings { get; set; }
    }
}
