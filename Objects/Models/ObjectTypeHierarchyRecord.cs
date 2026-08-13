using Newtonsoft.Json;

namespace OpenManage.Client.Objects.Models
{
    public sealed class ObjectTypeHierarchyRecord
    {
        [JsonProperty("objectType")]
        public int ObjectType { get; set; }

        [JsonProperty("parentTypeId")]
        public int ParentTypeId { get; set; }

        [JsonProperty("objectTypeName")]
        public string ObjectTypeName { get; set; }

        [JsonProperty("icon")]
        public byte[] Icon { get; set; }

        [JsonProperty("iconPlaceholder32x16")]
        public byte[] IconPlaceholder32x16 { get; set; }

        [JsonProperty("iconPlaceholder64x32")]
        public byte[] IconPlaceholder64x32 { get; set; }
    }
}
