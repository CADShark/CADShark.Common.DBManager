using Newtonsoft.Json;

namespace OpenVault.Client
{
public class SearchResponse
{
    [JsonProperty("objectIds")] public int[] ObjectIds { get; set; }
}
}