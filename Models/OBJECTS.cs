namespace CADShark.Common.DBManager.Models
{
    public class OBJECTS
    {
        public int F_ID { get; set; }
        public int F_OBJECT_TYPES_ID { get; set; }
        public int F_CAPTION { get; set; }
        public int F_OBJECT_CREATE { get; set; }
        public virtual OBJECT_TYPES OBJECTTYPES { get; set; }
    }
}