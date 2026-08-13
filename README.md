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

## Read and update an object

```csharp
var objectDetails = await client.Objects.GetByIdAsync(
    objectId,
    cancellationToken);

var attributeById = await client.Objects.GetAttributeByIdAsync(
    objectId,
    attributeId: 2001,
    cancellationToken);

var attributeByName = await client.Objects.GetAttributeByNameAsync(
    objectId,
    attributeName: "Designation",
    cancellationToken);

var updated = await client.Objects.UpdateAttributeAsync(
    objectId,
    attributeId: 2001,
    value: "A-002",
    cancellationToken);
```

The current OpenVault API exposes individual attribute lookup by ID or name. It does not expose an endpoint that returns every attribute value for an object.

Attribute values can be removed independently from their metadata definition:

```csharp
await client.Objects.DeleteAttributeAsync(
    objectId,
    attributeId: 2001,
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

## End-to-end object flow

```csharp
var created = await client.Objects.CreateAsync(objectType, cancellationToken);

try
{
    await client.Objects.AddAttributeAsync(
        created.ObjectId,
        attributeId,
        "SDK-SMOKE-001",
        cancellationToken);

    var foundIds = await client.Search.SearchAsync(searchRequest, cancellationToken);
    var readBack = await client.Objects.GetByIdAsync(created.ObjectId, cancellationToken);
    var readAttribute = await client.Objects.GetAttributeByIdAsync(
        created.ObjectId,
        attributeId,
        cancellationToken);
}
finally
{
    await client.Objects.DeleteAsync(created.ObjectId, cancellationToken);
}
```

Use only a test object type and test metadata IDs when running this flow against a shared server.

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


## Create a SolidWorks composition

Object relations are exposed through `client.Relations`. A relation is attached to a physical parent version and points to the logical child object. OpenVault resolves the child version when the composition is read.

```csharp
var relation = await client.Relations.CreateAsync(
    parentObjectId: assembly.ObjectId,
    childObjectId: part.ObjectId,
    relationType: 1014,
    cancellationToken);

var composition = await client.Objects.GetCompositionAsync(
    assembly.ObjectId,
    cancellationToken);
```

Relations can be moved or removed without deleting the related objects:

```csharp
await client.Relations.MoveAsync(
    relation.RelationId,
    newParentObjectId,
    cancellationToken);

await client.Relations.DeleteAsync(
    relation.RelationId,
    cancellationToken);
```

The server rejects self-references, duplicate child relations under one parent version, and direct or indirect composition cycles.

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
