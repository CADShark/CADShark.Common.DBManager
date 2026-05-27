using Newtonsoft.Json;

namespace CADShark.Common.DBManager;

public class SearchResponse
{
    [JsonProperty("objectIds")] public int[] ObjectIds { get; set; }
}
