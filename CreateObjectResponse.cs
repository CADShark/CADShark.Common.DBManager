using Newtonsoft.Json;

namespace OpenManage.Client.Objects.Models
{
    public sealed class ObjectResponse
    {
        [JsonProperty("objectId")]
        public long ObjectId { get; set; }

        [JsonProperty("versionId")]
        public int VersionId { get; set; }

        [JsonProperty("objectType")]
        public int ObjectType { get; set; }
    }
}
