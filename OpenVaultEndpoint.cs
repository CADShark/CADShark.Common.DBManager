namespace CADShark.Common.DBManager;

internal static class OpenVaultEndpoint
{
    public const string Objects = "api/objects";
    public const string ObjectSearch = "api/objects/search";
    public const string Storage = "api/storage";

    public static string ObjectAttributes(int objectId)
    {
        return $"api/objects/{objectId}/attributes";
    }
}
