using CADShark.Common.DBManager.Models;
using System.Data.Entity;

namespace CADShark.Common.DBManager.Data
{
    public class AddDbContext() : DbContext(ConnectionString)
    {
        private const string ConnectionString =
            "Server=SRVPDM;Database=OpenManager;User Id=sa;Password=P@$$w0rd;Encrypt=False;";

        //public DbSet<OBJECT_TYPES> ObjectTypes { get; set; }
        //public DbSet<OBJECTS> Objects { get; set; }
        public DbSet<Items> Items { get; set; }


        //public void test()
        //{
        //    var newItem = new Items
        //    {
        //        FileName = "file.FileName",
        //        Revision = "file.CurrentRevision",
        //        Version = 123
        //    };

        //    Items.Add(newItem);

        //    SaveChanges();
        //}
    }
}