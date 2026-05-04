using Newtonsoft.Json;

namespace CADShark.Common.DBManager;

public class CreateObjectRequest
{
    [JsonProperty("objectType")] public int ObjectType { get; set; }
}

public class SearchRequest
{
    public Filter[] Filters { get; set; }
}

public class Filter
{
    [JsonProperty("attributeId")] public int AttributeId { get; set; }

    [JsonProperty("value")] public string Value { get; set; }

    [JsonProperty("ObjectTypeID")] public int[]? ObjectTypeID { get; set; }
}

public class SearchResponse
{
    public int[] ObjectIds { get; set; }
}