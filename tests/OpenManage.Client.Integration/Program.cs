using OpenManage.Client;
using OpenManage.Client.Objects.Models;

const string baseUrl = "https://195.128.227.246:443/";
const int partObjectType = 1296;
const int assemblyObjectType = 1361;
const int designationAttributeId = 9;
const int nameAttributeId = 10;
const int compositionRelationType = 1014;

var documents = new[]
{
    new DocumentSpec("TEST-ASM-001", "Тестовая главная сборка", assemblyObjectType, null),
    new DocumentSpec("TEST-PRT-001", "Тестовая деталь 1", partObjectType, "TEST-ASM-001"),
    new DocumentSpec("TEST-PRT-002", "Тестовая деталь 2", partObjectType, "TEST-ASM-001"),
    new DocumentSpec("TEST-SUB-001", "Тестовая подсборка 1", assemblyObjectType, "TEST-ASM-001"),
    new DocumentSpec("TEST-PRT-003", "Тестовая деталь 3", partObjectType, "TEST-SUB-001"),
    new DocumentSpec("TEST-PRT-004", "Тестовая деталь 4", partObjectType, "TEST-SUB-001"),
    new DocumentSpec("TEST-SUB-002", "Тестовая подсборка 2", assemblyObjectType, "TEST-ASM-001"),
    new DocumentSpec("TEST-PRT-005", "Тестовая деталь 5", partObjectType, "TEST-SUB-002"),
    new DocumentSpec("TEST-PRT-006", "Тестовая деталь 6", partObjectType, "TEST-SUB-002")
};

using var client = new OpenManageClient(
    new OpenManageClientOptions
    {
        BaseAddress = new Uri(baseUrl),
        IgnoreServerCertificateErrors = true
    });

var createdObjects = new Dictionary<string, ObjectResponse>();

Console.WriteLine($"Server: {baseUrl}");
Console.WriteLine("Creating the SolidWorks document structure. Objects will be left on the server.");

foreach (var document in documents)
{
    var created = await client.Objects.CreateAsync(document.ObjectType);
    createdObjects.Add(document.Designation, created);

    Console.WriteLine(
        $"OBJECT {document.Designation}: ID={created.ObjectId}, TYPE={created.ObjectType}, VERSION={created.VersionId}");

    await client.Objects.AddAttributeAsync(
        created.ObjectId,
        designationAttributeId,
        document.Designation);

    await client.Objects.AddAttributeAsync(
        created.ObjectId,
        nameAttributeId,
        document.Name);
}

foreach (var document in documents.Where(x => x.ParentDesignation != null))
{
    var parent = createdObjects[document.ParentDesignation!];
    var child = createdObjects[document.Designation];

    var relation = await client.Relations.CreateAsync(
        parent.ObjectId,
        child.ObjectId,
        compositionRelationType);

    Console.WriteLine(
        $"RELATION ID={relation.RelationId}: {document.ParentDesignation} -> {document.Designation}, TYPE={relation.RelationType}");
}

var root = createdObjects["TEST-ASM-001"];
var composition = await client.Objects.GetCompositionAsync(root.ObjectId);
var expectedObjectIds = createdObjects.Values.Select(x => x.ObjectId).OrderBy(x => x).ToArray();
var actualObjectIds = composition.Select(x => x.ObjectId).Distinct().OrderBy(x => x).ToArray();

if (!expectedObjectIds.SequenceEqual(actualObjectIds))
{
    throw new InvalidOperationException(
        $"Composition mismatch. Expected [{string.Join(",", expectedObjectIds)}], " +
        $"received [{string.Join(",", actualObjectIds)}].");
}

foreach (var document in documents)
{
    var created = createdObjects[document.Designation];
    var designation = await client.Objects.GetAttributeByIdAsync(
        created.ObjectId,
        designationAttributeId);
    var name = await client.Objects.GetAttributeByIdAsync(
        created.ObjectId,
        nameAttributeId);

    if (designation.StringValue != document.Designation ||
        name.StringValue != document.Name)
    {
        throw new InvalidOperationException(
            $"Attribute verification failed for object {created.ObjectId}.");
    }
}

Console.WriteLine($"ROOT_OBJECT_ID={root.ObjectId}");
Console.WriteLine($"COMPOSITION_RECORDS={composition.Count}");
Console.WriteLine("SMOKE_TEST_RESULT=SUCCESS");
Console.WriteLine("All objects and relations were intentionally left on the server.");

internal sealed record DocumentSpec(
    string Designation,
    string Name,
    int ObjectType,
    string? ParentDesignation);
