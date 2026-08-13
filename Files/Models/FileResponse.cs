using System;

namespace OpenManage.Client.Files.Models
{
    public sealed class FileResponse
    {
        public int FileId { get; set; }
        public string FileName { get; set; }
        public int FileSize { get; set; }
        public DateTime? FileDate { get; set; }
        public long? ObjectLinkId { get; set; }
        public int AttributeId { get; set; }
        public int LinkType { get; set; }
    }
}
