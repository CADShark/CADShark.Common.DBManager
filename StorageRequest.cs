using Newtonsoft.Json;

namespace CADShark.Common.DBManager;

public class StorageRequest
{
    [JsonProperty("fileName")] public string FileName { get; set; }
    [JsonProperty("fileBody")] public string FileBody { get; set; }
    [JsonProperty("objectLinkId")] public int ObjectLinkId { get; set; }
    [JsonProperty("attributeId")] public int AttributeId { get; set; }
    [JsonProperty("linkType")] public int LinkType { get; set; }
}
