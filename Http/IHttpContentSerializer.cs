using System.Net.Http;
using System.Threading.Tasks;

namespace OpenVault.Client.Http
{
    internal interface IHttpContentSerializer
    {
        HttpContent CreateContent<T>(T value);
        Task<T> ReadAsync<T>(HttpContent content);
    }
}