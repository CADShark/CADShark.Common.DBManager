using Newtonsoft.Json;
using System.Collections.Generic;

namespace OpenManage.Client.Search.Models
{
    internal sealed class SearchObjectsResponse
    {
        [JsonProperty("objectIds")]
        public List<long> ObjectIds { get; set; }
    }
}
