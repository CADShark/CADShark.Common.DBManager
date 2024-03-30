using System;
using CADShark.Common.DBManager.Models;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;

namespace CADShark.Common.DBManager.Data
{
    public class AddDbContext(string connectionString) : DbContext(connectionString), IAddDbContext
    {
        public DbSet<Items> Items { get; set; }

        public bool IsConnectionValid()
        {
            try
            {
                Database.Connection.Open();
                Database.Connection.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void CreateDataBase(out string error)
        {
            error = null;
            try
            {
                if (!Database.Exists())
                {
                    Database.Create();
                }



            }
            catch (Exception e)
            {
                error = e.Message;
            }
        }

        public void Migrator()
        {
            var configuration = new DbContextConfiguration();
            var migrator = new DbMigrator(configuration);

            // Отримуємо список невиконаних міграцій
            var pendingMigrations = migrator.GetPendingMigrations().ToList();

            if (pendingMigrations.Any())
            {
                Console.WriteLine("Database migration is needed.");
                // Викликаємо міграцію
                migrator.Update();
                Console.WriteLine("Database migration completed successfully.");
            }
            else
            {
                Console.WriteLine("Database migration is not needed.");
            }

        }

        public new void SaveChanges()
        {
            base.SaveChanges();
        }
    }

    internal sealed class DbContextConfiguration : DbMigrationsConfiguration<AddDbContext>
    {
        public DbContextConfiguration()
        {
            AutomaticMigrationsEnabled = false;
        }
    }
}