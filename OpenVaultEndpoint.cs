namespace OpenManage.Client.Http
{
    internal static class OpenVaultEndpoint
    {
        public const string Objects = "api/objects";
        public const string ObjectSearch = "api/objects/search";

        public static string Object(long objectId)
        {
            return $"api/objects/{objectId}";
        }

        public static string ObjectAttributes(long objectId)
        {
            return $"api/objects/{objectId}/attributes";
        }
    }
}
