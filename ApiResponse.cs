using Newtonsoft.Json;

namespace CADShark.Common.DBManager;

public class ApiResponse<T>
{
    [JsonProperty("success")] public bool Success { get; set; }

    [JsonProperty("data")] public T Data { get; set; }

    [JsonProperty("error")] public string Error { get; set; }
}