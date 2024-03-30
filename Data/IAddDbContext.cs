using CADShark.Common.DBManager.Models;
using System.Data.Entity;

namespace CADShark.Common.DBManager.Data
{
    public interface IAddDbContext
    {
        DbSet<Items> Items { get; set; }
        bool IsConnectionValid();

        void CreateDataBase(out string error);

        void Migrator();

        void SaveChanges();
    }
}