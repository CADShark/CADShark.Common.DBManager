using System;
using System.Collections.Generic;

namespace OpenManage.Client.Mapping
{
    public sealed class EngineeringPropertyMapper : IEngineeringPropertyMapper
    {
        public const int RelativePathAttributeId = 1038;

        public IReadOnlyList<OpenVaultAttributeValue> Map(
            IDictionary<string, string> properties,
            IEnumerable<PropertyAttributeMapping> mappings,
            string relativePath)
        {
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));
            if (mappings == null)
                throw new ArgumentNullException(nameof(mappings));
            if (string.IsNullOrWhiteSpace(relativePath))
                throw new ArgumentException("Relative path cannot be empty.", nameof(relativePath));

            var values = new List<OpenVaultAttributeValue>();
            var attributeIds = new HashSet<int>();

            foreach (var mapping in mappings)
            {
                if (mapping == null)
                    continue;
                if (string.IsNullOrWhiteSpace(mapping.PropertyName))
                    throw new ArgumentException("Mapping property name cannot be empty.", nameof(mappings));
                if (mapping.AttributeId <= 0)
                    throw new ArgumentException("Mapping attribute ID must be positive.", nameof(mappings));
                if (mapping.AttributeId == RelativePathAttributeId)
                    throw new ArgumentException(
                        "Attribute 1038 is reserved for the workspace-relative path.",
                        nameof(mappings));
                if (!attributeIds.Add(mapping.AttributeId))
                    throw new ArgumentException(
                        "Each OpenVault attribute ID can be mapped only once.",
                        nameof(mappings));

                string value;
                if (!properties.TryGetValue(mapping.PropertyName, out value))
                    continue;

                values.Add(new OpenVaultAttributeValue
                {
                    AttributeId = mapping.AttributeId,
                    Value = value ?? string.Empty
                });
            }

            values.Add(new OpenVaultAttributeValue
            {
                AttributeId = RelativePathAttributeId,
                Value = relativePath
            });

            return values;
        }
    }
}
