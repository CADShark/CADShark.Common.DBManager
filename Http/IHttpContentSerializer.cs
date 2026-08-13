using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace OpenManage.Client.Http
{
    internal interface IHttpContentSerializer
    {
        HttpContent CreateContent<T>(T value);

        Task<T> ReadAsync<T>(
            HttpContent content,
            CancellationToken cancellationToken);
    }
}
