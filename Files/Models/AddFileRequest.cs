namespace OpenManage.Client.Files.Models
{
    public sealed class AddFileRequest
    {
        public string FileName { get; set; }
        public byte[] FileBody { get; set; }
        public long ObjectLinkId { get; set; }
        public int AttributeId { get; set; }
        public int LinkType { get; set; }
    }
}
