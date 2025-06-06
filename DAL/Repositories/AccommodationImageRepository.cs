using Microsoft.Data.SqlClient;
using DAL.Models;
using DAL.Interfaces;

public class AccommodationImageRepository : IAccommodationImageRepository
{
    private readonly string _connectionString;

    public AccommodationImageRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<List<AccommodationImage>> GetByAccommodationIdAsync(int accommodationId)
    {
        var images = new List<AccommodationImage>();

        using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        var query = "SELECT ImageUrl FROM AccommodationImage WHERE AccommodationId = @AccommodationId";
        using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@AccommodationId", accommodationId);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            images.Add(new AccommodationImage
            {
                AccommodationId = accommodationId,
                ImageUrl = reader.GetString(0)
            });
        }

        return images;
    }
}
