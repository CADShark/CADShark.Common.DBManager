using Newtonsoft.Json;

namespace OpenManage.Client.Objects.Models
{
    public sealed class CreateObjectRequest
    {
        [JsonProperty("objectType")]
        public int ObjectType { get; set; }

        [JsonProperty("versionId")]
        public int? VersionId { get; set; }
    }
}
