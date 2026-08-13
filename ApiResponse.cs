using Newtonsoft.Json;

namespace OpenManage.Client.Contracts
{
    public sealed class ApiResponse<T>
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }

        [JsonProperty("error")]
        public ApiError Error { get; set; }
    }

    public sealed class ApiError
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
