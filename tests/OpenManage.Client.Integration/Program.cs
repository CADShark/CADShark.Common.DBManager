using OpenManage.Client;

const string baseUrl = "https://195.128.227.246:443/";
const int objectType = 1296;
const int designationAttributeId = 9;
const int nameAttributeId = 10;
const string designationValue = "Обозначение";
const string nameValue = "Наименование";

using var client = new OpenManageClient(
    new OpenManageClientOptions
    {
        BaseAddress = new Uri(baseUrl),
        IgnoreServerCertificateErrors = true
    });

Console.WriteLine($"Server: {baseUrl}");
Console.WriteLine($"Creating ObjectType {objectType}...");

var created = await client.Objects.CreateAsync(objectType);
Console.WriteLine($"OBJECT_ID={created.ObjectId}");
Console.WriteLine($"VERSION_ID={created.VersionId}");

await client.Objects.AddAttributeAsync(
    created.ObjectId,
    designationAttributeId,
    designationValue);
Console.WriteLine($"Added attribute {designationAttributeId}: {designationValue}");

await client.Objects.AddAttributeAsync(
    created.ObjectId,
    nameAttributeId,
    nameValue);
Console.WriteLine($"Added attribute {nameAttributeId}: {nameValue}");

var readObject = await client.Objects.GetByIdAsync(created.ObjectId);
var designation = await client.Objects.GetAttributeByIdAsync(
    created.ObjectId,
    designationAttributeId);
var name = await client.Objects.GetAttributeByIdAsync(
    created.ObjectId,
    nameAttributeId);

if (readObject.ObjectId != created.ObjectId ||
    readObject.ObjectType != objectType ||
    designation.AttributeId != designationAttributeId ||
    designation.StringValue != designationValue ||
    name.AttributeId != nameAttributeId ||
    name.StringValue != nameValue)
{
    throw new InvalidOperationException("The OpenVault smoke-test response did not match the requested object and attributes.");
}

Console.WriteLine("SMOKE_TEST_RESULT=SUCCESS");
Console.WriteLine("The object was intentionally left on the server.");
