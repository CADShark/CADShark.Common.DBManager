# OpenManage.Client

OpenManage.Client is a reusable .NET client SDK for the OpenVault Web API. It centralizes HTTP, JSON serialization, API routes, response envelopes, cancellation and typed errors.

The library targets `netstandard2.0` so it can be consumed by .NET Framework 4.8 integrations and modern .NET applications, including .NET 10.

## Create a client

```csharp
using OpenManage.Client;

var client = new OpenManageClient(
    new OpenManageClientOptions
    {
        BaseAddress = new Uri("https://openvault.example/")
    });
```

The host application must provide `BaseAddress`. Certificate validation is enabled by default. `IgnoreServerCertificateErrors` must only be enabled explicitly for controlled development environments.

An application-managed `HttpClient` can also be supplied:

```csharp
var client = new OpenManageClient(httpClient);
```

In that case, disposing `OpenManageClient` does not dispose the supplied `HttpClient`.

## Create an object

```csharp
var created = await client.Objects.CreateAsync(
    objectType: 1296,
    cancellationToken);

long objectId = created.ObjectId;
int versionId = created.VersionId;
```

Object type values belong to the host integration. The SDK contains no SOLIDWORKS-specific IDs or behavior.

## Add an attribute

```csharp
var attribute = await client.Objects.AddAttributeAsync(
    objectId,
    attributeId: 2001,
    value: "A-001",
    cancellationToken);
```

## Search

```csharp
using OpenManage.Client.Search.Models;

var objectIds = await client.Search.SearchAsync(
    new SearchObjectsRequest
    {
        VersionId = null,
        Filters = new List<AttributeFilter>
        {
            new AttributeFilter
            {
                AttributeId = 2001,
                Value = "A-001",
                ObjectTypeIds = new List<int> { 1296 }
            }
        }
    },
    cancellationToken);
```

Object IDs are represented as `long`.

## Read hierarchy, navigator and composition

```csharp
var hierarchy = await client.Objects.GetHierarchyAsync(cancellationToken);

var navigatorRecords = await client.Objects.GetNavigatorRecordsAsync(
    objectType,
    cancellationToken);

var composition = await client.Objects.GetCompositionAsync(
    objectId,
    cancellationToken);
```

The corresponding models are:

- `ObjectTypeHierarchyRecord`
- `ObjectNavigatorRecord`
- `ObjectCompositionRecord`

Icons returned by the API are deserialized from JSON base64 values into `byte[]`. Composition relation and parent identifiers are nullable because the root record can have no relation or parent.

## Error handling

HTTP errors, backend business errors, network failures and invalid responses are exposed through `OpenManageApiException`:

```csharp
using OpenManage.Client.Http;

try
{
    await client.Objects.CreateAsync(objectType, cancellationToken);
}
catch (OpenManageApiException exception)
{
    // exception.ErrorKind
    // exception.StatusCode
    // exception.ErrorCode
    // exception.ResponseBody
}
```

## Build and test

```text
dotnet restore
dotnet build
dotnet test
```
