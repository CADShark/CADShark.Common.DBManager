using Newtonsoft.Json;

namespace OpenManage.Client.Relations.Models
{
    public sealed class CreateObjectRelationRequest
    {
        [JsonProperty("parentObjectId")]
        public long ParentObjectId { get; set; }

        [JsonProperty("childObjectId")]
        public long ChildObjectId { get; set; }

        [JsonProperty("relationType")]
        public int RelationType { get; set; }
    }
}
