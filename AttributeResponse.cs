using Newtonsoft.Json;

namespace OpenManage.Client.Objects.Models
{
    public sealed class AttributeResponse
    {
        [JsonProperty("attributeId")]
        public int AttributeId { get; set; }

        [JsonProperty("stringValue")]
        public string StringValue { get; set; }
    }
}
