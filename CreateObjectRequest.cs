using Newtonsoft.Json;

namespace CADShark.Common.DBManager;

public class CreateObjectRequest
{
    [JsonProperty("objectType")] public int ObjectType { get; set; }
}
