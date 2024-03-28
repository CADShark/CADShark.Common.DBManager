using System;

namespace CADShark.Common.DBManager.Models
{
    public class OBJECT_TYPES
    {
        public int F_OBJECT_TYPE { get; set; }
        public string F_OBJ_TYPE_NAME { get; set; }
        public string F_OBJ_NAME { get; set; }
        public byte[]? F_ICON { get; set; }
        public string? F_NOTE { get; set; }
        public Guid F_GUID { get; set; }
        public int F_VERSIONABLE { get; set; }
        public int F_DEFAULT_RELATIO { get; set; }
    }
}