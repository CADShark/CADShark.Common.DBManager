using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CADShark.Common.DBManager;

public class OpenVaultApi
{
    private readonly HttpClient _client;

    public OpenVaultApi()
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true
        };

        _client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://192.168.1.109:443/")
        };
    }

    public async Task<int> CreateObjectAsync(int objectType)
    {
        const string url = "api/objects";

        var request = new CreateObjectRequest
        {
            ObjectType = objectType
        };

        var json = JsonConvert.SerializeObject(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        var resultJson = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<ApiResponse<CreateObjectResponse>>(resultJson);

        return result.Data.ObjectId;
    }

    // ========= SEARCH OBJECTS =========
    public async Task<int[]> SearchObjectsAsync(SearchRequest request)
    {
        const string url = "api/objects/search";

        var json = JsonConvert.SerializeObject(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        var resultJson = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<ApiResponse<SearchResponse>>(resultJson);

        return result?.Data?.ObjectIds ?? [];
    }

    // ========= CREATE ATTRIBUTE =========
    public async Task<string> AddAttribute(int objectId, int attributeId, string value)
    {
        var url = $"api/objects/{objectId}/attributes";

        var request = new AttributeRequest
        {
            AttributeId = attributeId,

            StringValue = value
        };

        var json = JsonConvert.SerializeObject(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> WritteBlob(string fileName, byte[] fileBody, int objectLinkId, int attributeId,
        int linkType)
    {
        var url = "api/storage";

        var request = new StorageRequest
        {
            FileName = fileName,
            FileBody = Convert.ToBase64String(fileBody),
            ObjectLinkId = objectLinkId,
            AttributeId = attributeId,
            LinkType = linkType
        };

        var json = JsonConvert.SerializeObject(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}