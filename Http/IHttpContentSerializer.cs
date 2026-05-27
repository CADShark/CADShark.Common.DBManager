using System.Net.Http;
using System.Threading.Tasks;

namespace CADShark.Common.DBManager.Http;

internal interface IHttpContentSerializer
{
    HttpContent CreateContent<T>(T value);
    Task<T> ReadAsync<T>(HttpContent content);
}
