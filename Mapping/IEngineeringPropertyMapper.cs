using System.Collections.Generic;

namespace OpenManage.Client.Mapping
{
    public interface IEngineeringPropertyMapper
    {
        IReadOnlyList<OpenVaultAttributeValue> Map(
            IDictionary<string, string> properties,
            IEnumerable<PropertyAttributeMapping> mappings,
            string relativePath);
    }
}
