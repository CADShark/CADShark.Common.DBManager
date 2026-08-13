namespace OpenManage.Client.Http
{
    internal static class OpenVaultEndpoint
    {
        public const string Objects = "api/objects";
        public const string ObjectSearch = "api/objects/search";
        public const string ObjectHierarchy = "api/objects/hierarchy";

        public static string Object(long objectId)
        {
            return $"api/objects/{objectId}";
        }

        public static string ObjectAttributes(long objectId)
        {
            return $"api/objects/{objectId}/attributes";
        }

        public static string ObjectAttribute(long objectId, int attributeId)
        {
            return $"api/objects/{objectId}/attributes/{attributeId}";
        }

        public static string ObjectAttributeByName(long objectId, string attributeName)
        {
            return $"api/objects/{objectId}/attributes/by-name/{System.Uri.EscapeDataString(attributeName)}";
        }

        public static string ObjectNavigator(int objectType)
        {
            return $"api/objects/navigator/{objectType}";
        }

        public static string ObjectComposition(long objectId)
        {
            return $"api/objects/{objectId}/composition";
        }
    }
}
