using Microsoft.Data.SqlClient;
using DAL.Models;
using DAL.Interfaces;

namespace DAL.Repositories
{
    public class LandlordRepository : ILandlordRepository
    {
        private readonly string _connectionString;

        public LandlordRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Landlord?> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "SELECT LandlordId, UserId, Name, Email, PhoneNumber, CompanyName, TaxIdentificationNumber, IsVerified, VerificationDate, CreatedAt, UpdatedAt FROM Landlord WHERE LandlordId = @Id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Landlord
                {
                    LandlordId = reader.GetInt32(0),
                    UserId = reader.GetString(1),
                    Name = reader.GetString(2),
                    Email = reader.GetString(3),
                    PhoneNumber = reader.GetString(4),
                    CompanyName = reader.IsDBNull(5) ? null : reader.GetString(5),
                    TaxIdentificationNumber = reader.IsDBNull(6) ? null : reader.GetString(6),
                    IsVerified = reader.GetBoolean(7),
                    VerificationDate = reader.IsDBNull(8) ? (DateTime?)null : reader.GetDateTime(8),
                    CreatedAt = reader.GetDateTime(9),
                    UpdatedAt = reader.GetDateTime(10)
                };

            }

            return null;
        }

        public async Task<IEnumerable<Landlord>> GetAllAsync()
        {
            var landlords = new List<Landlord>();
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "SELECT LandlordId, Name, Email, UserId, IsVerified FROM Landlord";
            using var cmd = new SqlCommand(query, conn);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                landlords.Add(new Landlord
                {
                    LandlordId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Email = reader.GetString(2),
                    UserId = reader.GetString(3),
                    IsVerified = reader.GetBoolean(4)
                });
            }

            return landlords;
        }

        public async Task AddAsync(Landlord landlord)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = @"
            INSERT INTO Landlord 
            (UserId, Name, Email, PhoneNumber, CompanyName, TaxIdentificationNumber, IsVerified, VerificationDate, CreatedAt, UpdatedAt)
            VALUES 
            (@UserId, @Name, @Email, @PhoneNumber, @CompanyName, @TaxId, @IsVerified, @VerificationDate, @CreatedAt, @UpdatedAt)";

            using var cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@UserId", landlord.UserId);
            cmd.Parameters.AddWithValue("@Name", landlord.Name);
            cmd.Parameters.AddWithValue("@Email", landlord.Email);
            cmd.Parameters.AddWithValue("@PhoneNumber", landlord.PhoneNumber);
            cmd.Parameters.AddWithValue("@CompanyName", (object?)landlord.CompanyName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TaxId", (object?)landlord.TaxIdentificationNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsVerified", landlord.IsVerified);
            cmd.Parameters.AddWithValue("@VerificationDate", (object?)landlord.VerificationDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CreatedAt", landlord.CreatedAt);
            cmd.Parameters.AddWithValue("@UpdatedAt", landlord.UpdatedAt);

            await cmd.ExecuteNonQueryAsync();
        }


        public async Task UpdateAsync(Landlord landlord)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = @"
            UPDATE Landlord
            SET 
                Name = @Name,
                Email = @Email,
                UserId = @UserId,
                PhoneNumber = @PhoneNumber,
                CompanyName = @CompanyName,
                TaxIdentificationNumber = @TaxId,
                IsVerified = @IsVerified,
                VerificationDate = @VerificationDate,
                UpdatedAt = @UpdatedAt
            WHERE LandlordId = @LandlordId";

            using var cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@Name", landlord.Name);
            cmd.Parameters.AddWithValue("@Email", landlord.Email);
            cmd.Parameters.AddWithValue("@UserId", landlord.UserId);
            cmd.Parameters.AddWithValue("@PhoneNumber", landlord.PhoneNumber);
            cmd.Parameters.AddWithValue("@CompanyName", (object?)landlord.CompanyName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TaxId", (object?)landlord.TaxIdentificationNumber ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsVerified", landlord.IsVerified);
            cmd.Parameters.AddWithValue("@VerificationDate", (object?)landlord.VerificationDate ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@UpdatedAt", landlord.UpdatedAt);
            cmd.Parameters.AddWithValue("@LandlordId", landlord.LandlordId);

            await cmd.ExecuteNonQueryAsync();
        }


        public async Task DeleteAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "DELETE FROM Landlord WHERE LandlordId = @Id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "SELECT 1 FROM Landlord WHERE LandlordId = @Id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            var result = await cmd.ExecuteScalarAsync();
            return result != null;
        }

        public async Task<Landlord> GetByIdWithPropertiesAsync(int id)
        {
            var landlord = await GetByIdAsync(id);
            if (landlord == null) return null;

            landlord.Accommodations = new List<Accommodation>();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "SELECT AccommodationId, Title, Description, UniversityId, AccommodationTypeId, IsAvailable FROM Accommodation WHERE LandlordId = @LandlordId";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@LandlordId", id);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                landlord.Accommodations.Add(new Accommodation
                {
                    AccommodationId = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Description = reader.GetString(2),
                    UniversityId = reader.GetInt32(3),
                    AccommodationTypeId = reader.GetInt32(4),
                    IsAvailable = reader.GetBoolean(5),
                    LandlordId = id
                });
            }

            return landlord;
        }

        public async Task<IEnumerable<Landlord>> GetByUniversityAsync(int universityId)
        {
            var landlords = new List<Landlord>();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = @"
                SELECT DISTINCT l.LandlordId, l.Name, l.Email, l.UserId, l.IsVerified
                FROM Landlord l
                INNER JOIN Accommodation a ON l.LandlordId = a.LandlordId
                WHERE a.UniversityId = @UniversityId";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UniversityId", universityId);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                landlords.Add(new Landlord
                {
                    LandlordId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Email = reader.GetString(2),
                    UserId = reader.GetString(3),
                    IsVerified = reader.GetBoolean(4)
                });
            }

            return landlords;
        }

        public async Task<IEnumerable<Landlord>> GetActiveLandlordsAsync()
        {
            var landlords = new List<Landlord>();
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "SELECT LandlordId, Name, Email, UserId, IsVerified FROM Landlord WHERE IsVerified = 1";
            using var cmd = new SqlCommand(query, conn);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                landlords.Add(new Landlord
                {
                    LandlordId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Email = reader.GetString(2),
                    UserId = reader.GetString(3),
                    IsVerified = reader.GetBoolean(4)
                });
            }

            return landlords;
        }

        public async Task<Landlord?> GetByUserIdAsync(string userId)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "SELECT LandlordId, Name, Email, UserId, IsVerified FROM Landlord WHERE UserId = @UserId";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserId", userId);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Landlord
                {
                    LandlordId = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Email = reader.GetString(2),
                    UserId = reader.GetString(3),
                    IsVerified = reader.GetBoolean(4)
                };
            }

            return null;
        }

        public async Task<int> GetCountAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "SELECT COUNT(*) FROM Landlord";
            using var cmd = new SqlCommand(query, conn);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }
    }
}
