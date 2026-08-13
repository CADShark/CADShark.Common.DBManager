using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace OpenManage.Client.Http
{
    internal sealed class JsonHttpContentSerializer : IHttpContentSerializer
    {
        private const string MediaType = "application/json";

        public HttpContent CreateContent<T>(T value)
        {
            var json = JsonConvert.SerializeObject(value);
            return new StringContent(json, Encoding.UTF8, MediaType);
        }

        public T Deserialize<T>(string content)
        {
            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}
