using Newtonsoft.Json;

namespace CADShark.Common.DBManager;

public class AttributeRequest
{
    [JsonProperty("attributeId")] public int AttributeId { get; set; }
    [JsonProperty("value")] public string StringValue { get; set; }
}
