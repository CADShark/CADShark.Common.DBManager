namespace OpenManage.Client.Integration.Models
{
    public sealed class CreateOnlyDocumentResult
    {
        public bool IsSuccess { get; set; }
        public long? ObjectId { get; set; }
        public int? VersionId { get; set; }
        public int? FileId { get; set; }
        public int AddedAttributeCount { get; set; }
        public string ErrorMessage { get; set; }
    }
}
