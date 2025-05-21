using System;
using System.Data.SqlClient;

namespace CADShark.Common.DBManager.Data
{
    public static class DatabaseInitializer
    {
        public static (bool Success, string Error) EnsureDatabaseAndTable(string connectionString, string databaseName)
        {
            try
            {
                var masterConnectionString = GetMasterConnectionString(connectionString);

                using (var conn = new SqlConnection(masterConnectionString))
                {
                    conn.Open();

                    var checkDbCmd = new SqlCommand($@"
                        IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = @dbname)
                        BEGIN
                            CREATE DATABASE [{databaseName}]
                        END", conn);

                    checkDbCmd.Parameters.AddWithValue("@dbname", databaseName);
                    checkDbCmd.ExecuteNonQuery();
                }

                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    var checkTableCmd = new SqlCommand(@"
                        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Items')
                        BEGIN
                            CREATE TABLE Items (
                                Id INT IDENTITY(1,1) PRIMARY KEY,
                                DocumentId INT NOT NULL,
                                FileName NVARCHAR(255),
                                Revision NVARCHAR(50),
                                Version INT,
                                Config NVARCHAR(255),
                                Blob VARBINARY(MAX),
                                DocType NVARCHAR(50)
                            )
                        END", conn);

                    checkTableCmd.ExecuteNonQuery();
                }

                return (true, null);
            }
            catch (ArgumentException ex)
            {
                return (false, "Ошибка в формате строки подключения: " + ex.Message);

            }
            catch (SqlException ex)
            {
                
                return (false, "Ошибка подключения к серверу SQL: " + ex.Message);
            }
            catch (Exception ex)
            {
                return (false, "Ошибка при подключении или создании базы: " + ex.Message);
            }
        }

        private static string GetMasterConnectionString(string originalConnStr)
        {
            var builder = new SqlConnectionStringBuilder(originalConnStr)
            {
                InitialCatalog = "master"
            };
            return builder.ConnectionString;
        }
    }
}
