using Newtonsoft.Json;

namespace CADShark.Common.DBManager;

public class SearchRequest
{
    [JsonProperty("filters")] public Filter[] Filters { get; set; }
}
