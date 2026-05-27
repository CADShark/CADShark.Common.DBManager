using CADShark.Common.DBManager.Http;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CADShark.Common.DBManager;

public class OpenVaultApi : IDisposable
{
    private readonly HttpClient _client;
    private readonly IHttpContentSerializer _serializer;
    private readonly bool _ownsClient;

    public OpenVaultApi()
        : this(new OpenVaultApiOptions())
    {
    }

    public OpenVaultApi(OpenVaultApiOptions options)
        : this(CreateClient(options), new JsonHttpContentSerializer(), true)
    {
    }

    public OpenVaultApi(HttpClient client)
        : this(client, new JsonHttpContentSerializer(), false)
    {
    }

    internal OpenVaultApi(HttpClient client, IHttpContentSerializer serializer, bool ownsClient)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _ownsClient = ownsClient;
    }

    public async Task<int> CreateObjectAsync(int objectType)
    {
        var request = new CreateObjectRequest
        {
            ObjectType = objectType
        };

        var result = await PostAsync<CreateObjectRequest, ApiResponse<CreateObjectResponse>>(
            OpenVaultEndpoint.Objects,
            request);

        return result.Data.ObjectId;
    }

    public async Task<int[]> SearchObjectsAsync(SearchRequest request)
    {
        var result = await PostAsync<SearchRequest, ApiResponse<SearchResponse>>(
            OpenVaultEndpoint.ObjectSearch,
            request);

        return result?.Data?.ObjectIds ?? [];
    }

    public Task<string> AddAttribute(int objectId, int attributeId, string value)
    {
        var request = new AttributeRequest
        {
            AttributeId = attributeId,
            StringValue = value
        };

        return PostForStringAsync(OpenVaultEndpoint.ObjectAttributes(objectId), request);
    }

    public Task<string> WriteBlobAsync(
        string fileName,
        byte[] fileBody,
        int objectLinkId,
        int attributeId,
        int linkType)
    {
        if (fileBody == null)
            throw new ArgumentNullException(nameof(fileBody));

        var request = new StorageRequest
        {
            FileName = fileName,
            FileBody = Convert.ToBase64String(fileBody),
            ObjectLinkId = objectLinkId,
            AttributeId = attributeId,
            LinkType = linkType
        };

        return PostForStringAsync(OpenVaultEndpoint.Storage, request);
    }

    public Task<string> WritteBlob(
        string fileName,
        byte[] fileBody,
        int objectLinkId,
        int attributeId,
        int linkType)
    {
        return WriteBlobAsync(fileName, fileBody, objectLinkId, attributeId, linkType);
    }

    public void Dispose()
    {
        if (_ownsClient)
            _client.Dispose();
    }

    private static HttpClient CreateClient(OpenVaultApiOptions options)
    {
        if (options == null)
            throw new ArgumentNullException(nameof(options));

        var handler = new HttpClientHandler();

        if (options.IgnoreServerCertificateErrors)
            handler.ServerCertificateCustomValidationCallback = (message, certificate, chain, errors) => true;

        return new HttpClient(handler)
        {
            BaseAddress = options.BaseAddress
        };
    }

    private async Task<TResponse> PostAsync<TRequest, TResponse>(string url, TRequest request)
    {
        var content = _serializer.CreateContent(request);
        var response = await _client.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        return await _serializer.ReadAsync<TResponse>(response.Content);
    }

    private async Task<string> PostForStringAsync<TRequest>(string url, TRequest request)
    {
        var content = _serializer.CreateContent(request);
        var response = await _client.PostAsync(url, content);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}
