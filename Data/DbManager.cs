using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CADShark.Common.DBManager.Models;

namespace CADShark.Common.DBManager.Data;

public class DbManager : IDbManager
{
    private readonly string _connectionString;

    public DbManager(string connectionString)
    {
        _connectionString = connectionString;
    }
    public void Add(Items item)
    {
        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(@"
            INSERT INTO Items (DocumentId, FileName, Revision, Version, Config, Blob, DocType)
            VALUES (@DocumentId, @FileName, @Revision, @Version, @Config, @Blob, @DocType)", connection);
        command.Parameters.AddWithValue("@DocumentId", item.DocumentId);
        command.Parameters.AddWithValue("@FileName", item.FileName ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@Revision", item.Revision ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@Version", item.Version);
        command.Parameters.AddWithValue("@Config", item.Config ?? (object)DBNull.Value);
        command.Parameters.Add("@Blob", SqlDbType.VarBinary).Value = item.Blob ?? (object)DBNull.Value;
        command.Parameters.AddWithValue("@DocType", item.DocType ?? (object)DBNull.Value);

        connection.Open();
        command.ExecuteNonQuery();
    }

    public IEnumerable<Items> Items()
    {
        var items = new List<Items>();

        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        var command = new SqlCommand(@"
            SELECT Id, DocumentId, FileName, Revision, Version, Config, DocType
            FROM Items", connection);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            items.Add(new Items
            {
                Id = reader.GetInt32(0),
                DocumentId = reader.GetInt32(1),
                FileName = reader.GetString(2),
                Revision = reader.GetString(3),
                Version = reader.GetInt32(4),
                Config = reader.IsDBNull(5) ? null : reader.GetString(5),
                DocType = reader.GetString(6),
                Blob = null
            });
        }

        return items;
    }

    public byte[] GetBlobById(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        var command = new SqlCommand("SELECT Blob FROM Items WHERE Id = @Id", connection);
        command.Parameters.AddWithValue("@Id", id);

        var result = command.ExecuteScalar();
        return result as byte[];
    }

    public void UpdateItem(Items item)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand(@"
                UPDATE Items 
                SET DocumentId = @DocumentId, FileName = @FileName, Revision = @Revision, 
                    Version = @Version, Config = @Config, Blob = @Blob, DocType = @DocType
                WHERE Id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", item.Id);
        cmd.Parameters.AddWithValue("@DocumentId", item.DocumentId);
        cmd.Parameters.AddWithValue("@FileName", item.FileName);
        cmd.Parameters.AddWithValue("@Revision", item.Revision);
        cmd.Parameters.AddWithValue("@Version", item.Version);
        cmd.Parameters.AddWithValue("@Config", (object)item.Config ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Blob", (object)item.Blob ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DocType", item.DocType);

        conn.Open();
        cmd.ExecuteNonQuery();
    }

    public void DeleteItem(int id)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("DELETE FROM Items WHERE Id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);

        conn.Open();
        cmd.ExecuteNonQuery();
    }

    public List<Items> SearchItems(string fileName = null, string docType = null)
    {
        var items = new List<Items>();
        var whereClauses = new List<string>();
        var parameters = new List<SqlParameter>();

        if (!string.IsNullOrEmpty(fileName))
        {
            whereClauses.Add("FileName LIKE @FileName");
            parameters.Add(new SqlParameter("@FileName", "%" + fileName + "%"));
        }

        if (!string.IsNullOrEmpty(docType))
        {
            whereClauses.Add("DocType = @DocType");
            parameters.Add(new SqlParameter("@DocType", docType));
        }

        string where = whereClauses.Count > 0 ? "WHERE " + string.Join(" AND ", whereClauses) : "";

        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand($"SELECT * FROM Items {where}", conn);
        cmd.Parameters.AddRange(parameters.ToArray());

        conn.Open();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            items.Add(MapReaderToItem(reader));
        }

        return items;
    }

    private Items MapReaderToItem(SqlDataReader reader)
    {
        return new Items
        {
            Id = reader.GetInt32(reader.GetOrdinal("Id")),
            DocumentId = reader.GetInt32(reader.GetOrdinal("DocumentId")),
            FileName = reader.GetString(reader.GetOrdinal("FileName")),
            Revision = reader.GetString(reader.GetOrdinal("Revision")),
            Version = reader.GetInt32(reader.GetOrdinal("Version")),
            Config = reader["Config"] as string,
            Blob = reader["Blob"] as byte[],
            DocType = reader.GetString(reader.GetOrdinal("DocType"))
        };
    }
}