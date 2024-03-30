namespace CADShark.Common.DBManager.Models
{
    public class Items
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public string FileName { get; set; }
        public string Revision { get; set; }
        public int Version { get; set; }
        public string Config { get; set; }
        public byte[] Blob { get; set; }
        public string DocType { get; set; }

    }
}