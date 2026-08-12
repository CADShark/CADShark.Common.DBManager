using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OpenVault.Client.Http
{
    internal sealed class JsonHttpContentSerializer : IHttpContentSerializer
    {
        private const string MediaType = "application/json";

        public HttpContent CreateContent<T>(T value)
        {
            var json = JsonConvert.SerializeObject(value);
            return new StringContent(json, Encoding.UTF8, MediaType);
        }

        public async Task<T> ReadAsync<T>(HttpContent content)
        {
            var json = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}