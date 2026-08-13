# OpenVault Client API

This document describes the public API of the client library used to access OpenVault.

> The current assembly, root namespace and main type are named `OpenManage.Client` and
> `OpenManageClient`. The product API is OpenVault. Renaming the published .NET API is a
> separate compatibility decision and is not performed implicitly.

## Target platforms

The library targets `netstandard2.0` and can be referenced from:

- .NET Framework 4.8 integrations, including SOLIDWORKS;
- modern .NET applications, including .NET 10;
- other runtimes implementing .NET Standard 2.0.

## Creating the client

### Client-owned HttpClient

```csharp
using OpenManage.Client;
using System;

using (var client = new OpenManageClient(
    new OpenManageClientOptions
    {
        BaseAddress = new Uri("https://openvault.example/")
    }))
{
    // Use client.Objects, client.Relations, client.Search and client.Files.
}
```

`BaseAddress` is required and should end with `/`.

`IgnoreServerCertificateErrors` defaults to `false`. Enable it only in a controlled
development environment. It disables TLS certificate validation for the client-owned
`HttpClient`.

### Application-owned HttpClient

Use this overload when the host configures authentication, proxy settings, timeouts or
dependency injection:

```csharp
using OpenManage.Client;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

var httpClient = new HttpClient
{
    BaseAddress = new Uri("https://openvault.example/"),
    Timeout = TimeSpan.FromSeconds(60)
};

httpClient.DefaultRequestHeaders.Authorization =
    new AuthenticationHeaderValue("Bearer", accessToken);

using (var client = new OpenManageClient(httpClient))
{
    // OpenManageClient does not dispose an application-owned HttpClient.
}
```

## API groups

`OpenManageClient` is the facade exposed to consumers.

| Property | Interface | Purpose |
|---|---|---|
| `Objects` | `IObjectsClient` | Objects, attributes, hierarchy, navigator and composition |
| `Relations` | `IRelationsClient` | Create, move and delete object relations |
| `Search` | `ISearchClient` | Search objects by attribute filters |
| `Files` | `IFilesClient` | Add files through `/api/Storage` |

Every asynchronous method accepts an optional `CancellationToken`.

## IObjectsClient

Namespace: `OpenManage.Client.Objects`.

| Method | Result | OpenVault endpoint | Description |
|---|---|---|---|
| `CreateAsync(int objectType)` | `ObjectResponse` | `POST /api/objects` | Creates an object of the specified type |
| `GetByIdAsync(long objectId)` | `ObjectResponse` | `GET /api/objects/{objectId}` | Reads an object |
| `DeleteAsync(long objectId)` | — | `DELETE /api/objects/{objectId}` | Deletes an object |
| `AddAttributeAsync(long objectId, int attributeId, string value)` | `AttributeResponse` | `POST /api/objects/{objectId}/attributes` | Adds an attribute value |
| `GetAttributeByIdAsync(long objectId, int attributeId)` | `AttributeResponse` | `GET /api/objects/{objectId}/attributes/{attributeId}` | Reads an attribute by ID |
| `GetAttributeByNameAsync(long objectId, string attributeName)` | `AttributeResponse` | `GET /api/objects/{objectId}/attributes/by-name/{attributeName}` | Reads an attribute by name |
| `UpdateAttributeAsync(long objectId, int attributeId, string value)` | `AttributeResponse` | `PUT /api/objects/{objectId}/attributes` | Updates an attribute value |
| `DeleteAttributeAsync(long objectId, int attributeId)` | — | `DELETE /api/objects/{objectId}/attributes/{attributeId}` | Deletes an attribute value |
| `GetHierarchyAsync()` | `IReadOnlyList<ObjectTypeHierarchyRecord>` | `GET /api/objects/hierarchy` | Reads the object-type hierarchy |
| `GetNavigatorRecordsAsync(int objectType)` | `IReadOnlyList<ObjectNavigatorRecord>` | `GET /api/objects/navigator/{objectType}` | Reads navigator rows for a type |
| `GetCompositionAsync(long objectId)` | `IReadOnlyList<ObjectCompositionRecord>` | `GET /api/objects/{objectId}/composition` | Reads an object's composition |

The API currently supports reading an individual attribute by ID or name. It does not
provide a client method that returns every attribute value of an object.

### Object and attribute example

```csharp
const int solidWorksPartType = 1296;
const int designationAttributeId = 9;
const int nameAttributeId = 10;

var created = await client.Objects.CreateAsync(
    solidWorksPartType,
    cancellationToken);

await client.Objects.AddAttributeAsync(
    created.ObjectId,
    designationAttributeId,
    "SW-001",
    cancellationToken);

await client.Objects.AddAttributeAsync(
    created.ObjectId,
    nameAttributeId,
    "Bracket",
    cancellationToken);

var objectInfo = await client.Objects.GetByIdAsync(
    created.ObjectId,
    cancellationToken);

var designation = await client.Objects.GetAttributeByIdAsync(
    created.ObjectId,
    designationAttributeId,
    cancellationToken);
```

This sequence is not transactional. If an attribute call fails after object creation,
the object can remain partially populated.

## ISearchClient

Namespace: `OpenManage.Client.Search`.

| Method | Result | Endpoint |
|---|---|---|
| `SearchAsync(SearchObjectsRequest request)` | `IReadOnlyList<long>` | `POST /api/objects/search` |

```csharp
using OpenManage.Client.Search.Models;
using System.Collections.Generic;

var objectIds = await client.Search.SearchAsync(
    new SearchObjectsRequest
    {
        VersionId = null,
        Filters = new List<AttributeFilter>
        {
            new AttributeFilter
            {
                AttributeId = 9,
                Value = "SW-001",
                ObjectTypeIds = new List<int> { 1296 }
            }
        }
    },
    cancellationToken);
```

Object IDs are represented as `long`. Search-before-create is not part of the current
SOLIDWORKS first-stage workflow and will be introduced after the required server methods
are available.

## IRelationsClient

Namespace: `OpenManage.Client.Relations`.

| Method | Result | Endpoint | Description |
|---|---|---|---|
| `CreateAsync(long parentObjectId, long childObjectId, int relationType)` | `ObjectRelationResponse` | `POST /api/object-relations` | Creates a relation |
| `MoveAsync(long relationId, long newParentObjectId)` | `ObjectRelationResponse` | `POST /api/object-relations/{relationId}/move` | Moves a relation |
| `DeleteAsync(long relationId)` | — | `DELETE /api/object-relations/{relationId}` | Deletes a relation |

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

## IFilesClient

Namespace: `OpenManage.Client.Files`.

| Method | Result | Endpoint |
|---|---|---|
| `AddAsync(AddFileRequest request)` | `FileResponse` | `POST /api/Storage` |

At the first implementation stage only adding a file is used. Search, read, update,
delete and version-management operations are deferred until the corresponding server
API is finalized.

```csharp
using OpenManage.Client.Files.Models;
using System.IO;

var file = await client.Files.AddAsync(
    new AddFileRequest
    {
        FileName = "Part1.sldprt",
        FileBody = File.ReadAllBytes(@"D:\Vault\Parts\Part1.sldprt"),
        ObjectLinkId = created.ObjectId,
        AttributeId = 1002,
        LinkType = 4
    },
    cancellationToken);
```

Current server limitation: `ObjectLinkId` is accepted only within the `Int32` range.
The client exposes it as `long` for the target architecture but rejects larger values
before sending the request. Unifying these ID types remains a server/client task.

## CAD/ECAD integration interfaces

These interfaces keep vendor adapters separate from reusable OpenVault logic.

### IEngineeringDocumentSource

Namespace: `OpenManage.Client.Integration`.

```csharp
Task<EngineeringDocumentInfo> GetActiveDocumentAsync(
    CancellationToken cancellationToken);
```

A SOLIDWORKS adapter implements this interface and returns the active document path,
kind, configuration and properties. Future Inventor, Altium Designer and EPLAN adapters
can provide their own implementations without adding vendor SDK dependencies to the
client library.

### IWorkspacePathService

```csharp
string WorkspaceRoot { get; }
WorkspaceDocumentInfo Resolve(string fullPath);
```

`WindowsWorkspacePathService` validates that a saved document is under the workspace
root and returns its file name and relative path. The default root is `D:\Vault\`.

### IEngineeringPropertyMapper

Namespace: `OpenManage.Client.Mapping`.

```csharp
IReadOnlyList<OpenVaultAttributeValue> Map(
    IDictionary<string, string> properties,
    IEnumerable<PropertyAttributeMapping> mappings,
    string relativePath);
```

`EngineeringPropertyMapper` maps CAD/ECAD property names to OpenVault attribute IDs.
Attribute `1038` is reserved for the workspace-relative path and is added automatically.

### ICreateOnlyDocumentService

```csharp
Task<CreateOnlyDocumentResult> CreateAsync(
    CreateOnlyDocumentRequest request,
    CancellationToken cancellationToken);
```

`CreateOnlyDocumentService` implements the approved first-stage sequence:

1. validate the workspace path;
2. read the local file before changing server state;
3. map document properties;
4. create the OpenVault object;
5. add mapped attributes;
6. upload the main CAD file.

Supported object mappings:

| Document kind | Object type |
|---|---:|
| SOLIDWORKS part | 1296 |
| SOLIDWORKS assembly | 1361 |

The main file uses attribute `1002` and link type `4`. Drawings, PDF files, search,
update, delete, rollback and version management are outside the first stage.

### Complete create-only example

```csharp
using OpenManage.Client.Integration;
using OpenManage.Client.Integration.Models;
using OpenManage.Client.Mapping;
using System.Collections.Generic;

var document = await documentSource.GetActiveDocumentAsync(cancellationToken);

var service = new CreateOnlyDocumentService(client);
var result = await service.CreateAsync(
    new CreateOnlyDocumentRequest
    {
        Document = document,
        WorkspaceRoot = @"D:\Vault\",
        PropertyMappings = new[]
        {
            new PropertyAttributeMapping
            {
                PropertyName = "Обозначение",
                AttributeId = 9
            },
            new PropertyAttributeMapping
            {
                PropertyName = "Наименование",
                AttributeId = 10
            }
        }
    },
    cancellationToken);

if (!result.IsSuccess)
{
    // ObjectId and AddedAttributeCount can be populated after a partial failure.
    throw new InvalidOperationException(result.ErrorMessage);
}
```

There is intentionally no automatic rollback in the create-only service. Inspect
`ObjectId`, `AddedAttributeCount` and `FileId` when diagnosing a partial failure.

## Error handling

Transport errors, non-success HTTP responses, backend business errors and invalid JSON
are represented by `OpenManageApiException`.

```csharp
using OpenManage.Client.Http;

try
{
    await client.Objects.CreateAsync(1296, cancellationToken);
}
catch (OpenManageApiException exception)
{
    // exception.ErrorKind
    // exception.StatusCode
    // exception.ErrorCode
    // exception.ResponseBody
}
catch (OperationCanceledException)
{
    // The caller cancelled the operation.
}
```

## Scalar and OpenAPI

[Scalar](https://scalar.com/) is suitable for interactive documentation of the
**OpenVault Web API**, provided that the server publishes an OpenAPI document. It should
be configured in the ASP.NET Core API project, next to OpenAPI generation, not inside
this .NET Standard client library.

Recommended documentation split:

- OpenAPI + Scalar on the server: HTTP endpoints, request/response schemas and interactive
  calls;
- this document: consumer-facing C# interfaces, orchestration rules, limitations and
  CAD/ECAD examples;
- XML documentation generated from C# source in a future step: IDE IntelliSense and
  generated .NET API reference.

Scalar will not discover client-only abstractions such as
`IEngineeringDocumentSource`, `IEngineeringPropertyMapper` or
`ICreateOnlyDocumentService`. Those require C# documentation.

## Maintenance rule

Update this document whenever a public interface, route, request/response contract,
fixed integration ID or first-stage limitation changes.
