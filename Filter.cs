using Newtonsoft.Json;

namespace OpenVault.Client
{
    public class Filter
    {
        [JsonProperty("attributeId")] public int AttributeId { get; set; }

        [JsonProperty("value")] public string Value { get; set; }

        [JsonProperty("ObjectTypeID")] public int[] ObjectTypeID { get; set; }
    }
}