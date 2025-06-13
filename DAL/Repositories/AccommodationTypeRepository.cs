using DAL.Interfaces;
using Domain.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class AccommodationTypeRepository : IAccommodationTypeRepository
    {
        private readonly string _connectionString;

        public AccommodationTypeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<AccommodationType>> GetAllAsync()
        {
            var list = new List<AccommodationType>();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "SELECT AccommodationTypeId, Name FROM AccommodationType";
            using var cmd = new SqlCommand(query, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new AccommodationType
                {
                    AccommodationTypeId = reader.GetInt32(0),
                    Name = reader.GetString(1)
                });
            }

            return list;
        }

        public async Task<AccommodationType?> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "SELECT AccommodationTypeId, Name FROM AccommodationType WHERE AccommodationTypeId = @Id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new AccommodationType
                {
                    AccommodationTypeId = reader.GetInt32(reader.GetOrdinal("AccommodationTypeId")),
                    Name = reader.GetString(reader.GetOrdinal("Name"))
                };
            }

            return null;
        }

    }
}
