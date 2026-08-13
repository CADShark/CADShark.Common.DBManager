using Newtonsoft.Json;
using System.Collections.Generic;

namespace OpenManage.Client.Search.Models
{
    public sealed class SearchObjectsRequest
    {
        [JsonProperty("filters")]
        public List<AttributeFilter> Filters { get; set; }

        [JsonProperty("versionId")]
        public int? VersionId { get; set; }
    }
}
