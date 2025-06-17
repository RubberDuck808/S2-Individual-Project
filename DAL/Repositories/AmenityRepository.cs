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

        public async Task AddAsync(int accommodationId, IEnumerable<int> amenityIds)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            foreach (var amenityId in amenityIds)
            {
                var cmd = new SqlCommand(
                    "INSERT INTO AccommodationAmenity (AccommodationId, AmenityId) VALUES (@AccommodationId, @AmenityId)",
                    conn);
                cmd.Parameters.AddWithValue("@AccommodationId", accommodationId);
                cmd.Parameters.AddWithValue("@AmenityId", amenityId);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public async Task UpdateAsync(int accommodationId, IEnumerable<int> amenityIds)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            using var transaction = conn.BeginTransaction();

            try
            {
                var deleteCmd = new SqlCommand("DELETE FROM AccommodationAmenity WHERE AccommodationId = @AccommodationId", conn, transaction);
                deleteCmd.Parameters.AddWithValue("@AccommodationId", accommodationId);
                await deleteCmd.ExecuteNonQueryAsync();

                foreach (var amenityId in amenityIds)
                {
                    var insertCmd = new SqlCommand(
                        "INSERT INTO AccommodationAmenity (AccommodationId, AmenityId) VALUES (@AccommodationId, @AmenityId)",
                        conn, transaction);
                    insertCmd.Parameters.AddWithValue("@AccommodationId", accommodationId);
                    insertCmd.Parameters.AddWithValue("@AmenityId", amenityId);
                    await insertCmd.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


    }
}
