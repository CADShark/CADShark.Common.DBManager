using Newtonsoft.Json;

namespace OpenVault.Client
{
public class SearchRequest
{
    [JsonProperty("filters")] public Filter[] Filters { get; set; }
}
}