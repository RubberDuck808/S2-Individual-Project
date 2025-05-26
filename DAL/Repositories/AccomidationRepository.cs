using System.Data.SqlClient;
using DAL.Models;
using DAL.Interfaces;

namespace DAL.Repositories
{
    public class AccommodationRepository : IAccommodationRepository
    {
        private readonly string _connectionString;

        public AccommodationRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Accommodation?> GetAccommodationWithDetailsAsync(int id)
        {
            return await GetByIdAsync(id);
        }

        public async Task<Accommodation?> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "SELECT * FROM Accommodation WHERE AccommodationId = @Id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return ReadAccommodation(reader);
            }

            return null;
        }

        public async Task<IEnumerable<Accommodation>> GetAllAsync()
        {
            var list = new List<Accommodation>();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "SELECT * FROM Accommodation";
            using var cmd = new SqlCommand(query, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(ReadAccommodation(reader));
            }

            return list;
        }

        public Task<IEnumerable<Accommodation>> FindAsync(System.Linq.Expressions.Expression<Func<Accommodation, bool>> predicate)
        {
            throw new NotSupportedException("FindAsync using LINQ expressions is not supported in ADO.NET repositories.");
        }

        public async Task AddAsync(Accommodation accommodation)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = @"
                INSERT INTO Accommodation 
                (Title, Description, MonthlyRent, Size, MaxOccupants, IsAvailable, LandlordId, AccommodationTypeId, UniversityId)
                VALUES 
                (@Title, @Description, @MonthlyRent, @Size, @MaxOccupants, @IsAvailable, @LandlordId, @AccommodationTypeId, @UniversityId)";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Title", accommodation.Title);
            cmd.Parameters.AddWithValue("@Description", accommodation.Description);
            cmd.Parameters.AddWithValue("@MonthlyRent", accommodation.MonthlyRent);
            cmd.Parameters.AddWithValue("@Size", accommodation.Size);
            cmd.Parameters.AddWithValue("@MaxOccupants", accommodation.MaxOccupants);
            cmd.Parameters.AddWithValue("@IsAvailable", accommodation.IsAvailable);
            cmd.Parameters.AddWithValue("@LandlordId", accommodation.LandlordId);
            cmd.Parameters.AddWithValue("@AccommodationTypeId", accommodation.AccommodationTypeId);
            cmd.Parameters.AddWithValue("@UniversityId", accommodation.UniversityId);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Accommodation accommodation)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = @"
                UPDATE Accommodation
                SET Title = @Title, Description = @Description, MonthlyRent = @MonthlyRent, Size = @Size,
                    MaxOccupants = @MaxOccupants, IsAvailable = @IsAvailable, LandlordId = @LandlordId,
                    AccommodationTypeId = @AccommodationTypeId, UniversityId = @UniversityId
                WHERE AccommodationId = @AccommodationId";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Title", accommodation.Title);
            cmd.Parameters.AddWithValue("@Description", accommodation.Description);
            cmd.Parameters.AddWithValue("@MonthlyRent", accommodation.MonthlyRent);
            cmd.Parameters.AddWithValue("@Size", accommodation.Size);
            cmd.Parameters.AddWithValue("@MaxOccupants", accommodation.MaxOccupants);
            cmd.Parameters.AddWithValue("@IsAvailable", accommodation.IsAvailable);
            cmd.Parameters.AddWithValue("@LandlordId", accommodation.LandlordId);
            cmd.Parameters.AddWithValue("@AccommodationTypeId", accommodation.AccommodationTypeId);
            cmd.Parameters.AddWithValue("@UniversityId", accommodation.UniversityId);
            cmd.Parameters.AddWithValue("@AccommodationId", accommodation.AccommodationId);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "DELETE FROM Accommodation WHERE AccommodationId = @Id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "SELECT 1 FROM Accommodation WHERE AccommodationId = @Id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            var result = await cmd.ExecuteScalarAsync();
            return result != null;
        }

        public async Task<IEnumerable<Accommodation>> GetAccommodationsByLandlordAsync(int landlordId)
        {
            var accommodations = new List<Accommodation>();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "SELECT * FROM Accommodation WHERE LandlordId = @LandlordId";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@LandlordId", landlordId);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                accommodations.Add(ReadAccommodation(reader));
            }

            return accommodations;
        }

        public async Task<IEnumerable<Accommodation>> GetAccommodationsByUniversityAsync(int universityId)
        {
            var accommodations = new List<Accommodation>();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "SELECT * FROM Accommodation WHERE UniversityId = @UniversityId";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UniversityId", universityId);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                accommodations.Add(ReadAccommodation(reader));
            }

            return accommodations;
        }

        public async Task<IEnumerable<Accommodation>> GetAvailableAccommodationsAsync()
        {
            var accommodations = new List<Accommodation>();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "SELECT * FROM Accommodation WHERE IsAvailable = 1";
            using var cmd = new SqlCommand(query, conn);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                accommodations.Add(ReadAccommodation(reader));
            }

            return accommodations;
        }

        private Accommodation ReadAccommodation(SqlDataReader reader)
        {
            return new Accommodation
            {
                AccommodationId = reader.GetInt32(reader.GetOrdinal("AccommodationId")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Description = reader.GetString(reader.GetOrdinal("Description")),
                MonthlyRent = reader.GetDecimal(reader.GetOrdinal("MonthlyRent")),
                Size = reader.GetDecimal(reader.GetOrdinal("Size")),
                MaxOccupants = reader.GetInt32(reader.GetOrdinal("MaxOccupants")),
                IsAvailable = reader.GetBoolean(reader.GetOrdinal("IsAvailable")),
                LandlordId = reader.GetInt32(reader.GetOrdinal("LandlordId")),
                AccommodationTypeId = reader.GetInt32(reader.GetOrdinal("AccommodationTypeId")),
                UniversityId = reader.GetInt32(reader.GetOrdinal("UniversityId"))
            };
        }
    }
}
