using Newtonsoft.Json;

namespace OpenManage.Client.Objects.Models
{
    public sealed class ObjectCompositionRecord
    {
        [JsonProperty("relationId")]
        public long? RelationId { get; set; }

        [JsonProperty("parentObjectId")]
        public long? ParentObjectId { get; set; }

        [JsonProperty("objectId")]
        public long ObjectId { get; set; }

        [JsonProperty("objectType")]
        public int ObjectType { get; set; }

        [JsonProperty("versionId")]
        public int VersionId { get; set; }

        [JsonProperty("icon")]
        public byte[] Icon { get; set; }

        [JsonProperty("iconPlaceholder32x16")]
        public byte[] IconPlaceholder32x16 { get; set; }

        [JsonProperty("designation")]
        public string Designation { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
