using Newtonsoft.Json;

namespace OpenVault.Client
{
    public class AttributeResponse
    {
        [JsonProperty("attributeId")] public int AttributeId { get; set; }
        [JsonProperty("value")] public string Value { get; set; }
    }
}