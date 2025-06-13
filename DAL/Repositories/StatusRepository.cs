using DAL.Interfaces;
using Domain.Models;
using Microsoft.Data.SqlClient;

public class StatusRepository : IStatusRepository
{
    private readonly string _connectionString;

    public StatusRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<string?> GetNameByIdAsync(int statusId)
    {
        using var conn = new SqlConnection(_connectionString);
        var cmd = new SqlCommand("SELECT Name FROM Status WHERE StatusId = @id", conn);
        cmd.Parameters.AddWithValue("@id", statusId);

        await conn.OpenAsync();
        var result = await cmd.ExecuteScalarAsync();
        return result?.ToString();
    }

    public async Task<int?> GetIdByNameAsync(string name)
    {
        using var conn = new SqlConnection(_connectionString);
        var cmd = new SqlCommand("SELECT StatusId FROM Status WHERE Name = @name", conn);
        cmd.Parameters.AddWithValue("@name", name);

        await conn.OpenAsync();
        var result = await cmd.ExecuteScalarAsync();
        return result == null ? null : Convert.ToInt32(result);
    }

    public async Task<List<Status>> GetAllAsync()
    {
        var list = new List<Status>();
        using var conn = new SqlConnection(_connectionString);
        var cmd = new SqlCommand("SELECT * FROM Status", conn);

        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new Status
            {
                StatusId = (int)reader["StatusId"],
                Name = reader["Name"].ToString() ?? ""
            });
        }

        return list;
    }


}
