using Newtonsoft.Json;

namespace OpenManage.Client.Relations.Models
{
    public sealed class MoveObjectRelationRequest
    {
        [JsonProperty("newParentObjectId")]
        public long NewParentObjectId { get; set; }
    }
}
