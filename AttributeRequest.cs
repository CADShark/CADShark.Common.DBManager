using Newtonsoft.Json;

namespace OpenVault.Client
{
    public class AttributeRequest
    {
        [JsonProperty("attributeId")] public int AttributeId { get; set; }
        [JsonProperty("value")] public string StringValue { get; set; }
    }
}