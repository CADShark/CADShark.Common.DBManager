using Newtonsoft.Json;
using System.Collections.Generic;

namespace OpenManage.Client.Search.Models
{
    public sealed class AttributeFilter
    {
        [JsonProperty("attributeId")]
        public int AttributeId { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("ObjectTypeID")]
        public List<int> ObjectTypeIds { get; set; }
    }
}
