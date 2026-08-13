using System.Net.Http;

namespace OpenManage.Client.Http
{
    internal interface IHttpContentSerializer
    {
        HttpContent CreateContent<T>(T value);

        T Deserialize<T>(string content);
    }
}
