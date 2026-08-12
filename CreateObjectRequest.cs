using Newtonsoft.Json;

namespace OpenVault.Client
{
    public class CreateObjectRequest
    {
        [JsonProperty("objectType")] public int ObjectType { get; set; }
    }
}
