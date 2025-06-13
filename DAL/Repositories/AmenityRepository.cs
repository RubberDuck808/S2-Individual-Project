using DAL.Interfaces;
using Domain.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class AmenityRepository : IAmenityRepository
    {
        private readonly string _connectionString;

        public AmenityRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Amenity>> GetAllAsync()
        {
            var amenities = new List<Amenity>();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "SELECT AmenityId, Name, IconName FROM Amenity";
            using var cmd = new SqlCommand(query, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                amenities.Add(new Amenity
                {
                    AmenityId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    IconName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2)
                });
            }


            return amenities;
        }


        public async Task<List<Amenity>> GetByAccommodationIdAsync(int accommodationId)
        {
            var amenities = new List<Amenity>();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = @"
        SELECT a.AmenityId, a.Name, a.IconName
        FROM Amenity a
        INNER JOIN AccommodationAmenity aa ON a.AmenityId = aa.AmenityId
        WHERE aa.AccommodationId = @AccommodationId";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@AccommodationId", accommodationId);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                amenities.Add(new Amenity
                {
                    AmenityId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    IconName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2)
                });
            }

            return amenities;
        }

    }
}
