using Newtonsoft.Json;
using System;

namespace OpenManage.Client.Relations.Models
{
    public sealed class ObjectRelationResponse
    {
        [JsonProperty("relationId")]
        public long RelationId { get; set; }

        [JsonProperty("parentObjectId")]
        public long ParentObjectId { get; set; }

        [JsonProperty("childLogicalId")]
        public long ChildLogicalId { get; set; }

        [JsonProperty("resolvedChildObjectId")]
        public long ResolvedChildObjectId { get; set; }

        [JsonProperty("resolvedChildVersionId")]
        public int ResolvedChildVersionId { get; set; }

        [JsonProperty("relationType")]
        public int RelationType { get; set; }

        [JsonProperty("createDate")]
        public DateTime CreateDate { get; set; }
    }
}
