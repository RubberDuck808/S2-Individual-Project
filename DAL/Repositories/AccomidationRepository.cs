using Microsoft.Data.SqlClient;
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

        public async Task<IEnumerable<(Accommodation accommodation, int applicationCount)>> GetWithApplicationCountsByLandlordIdAsync(int landlordId)
        {
            var results = new List<(Accommodation, int)>();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = @"
            SELECT a.*, COUNT(app.ApplicationId) AS ApplicationCount
            FROM Accommodation a
            LEFT JOIN Application app ON a.AccommodationId = app.AccommodationId
            WHERE a.LandlordId = @LandlordId
            GROUP BY 
            a.AccommodationId, a.Title, a.Description, a.Address, a.PostCode, a.City, a.Country, 
            a.MonthlyRent, a.Size, a.MaxOccupants, a.IsAvailable, a.LandlordId, a.AccommodationTypeId,
            a.UniversityId, a.AvailableFrom";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@LandlordId", landlordId);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var accommodation = ReadAccommodation(reader);
                int applicationCount = reader.GetInt32(reader.GetOrdinal("ApplicationCount"));
                results.Add((accommodation, applicationCount));
            }

            return results;
        }

        public async Task<IEnumerable<Accommodation>> GetWithApplicationsByStudentIdAsync(int studentId)
        {
            var results = new List<Accommodation>();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = @"
            SELECT a.*
            FROM Accommodation a
            INNER JOIN Application app ON a.AccommodationId = app.AccommodationId
            WHERE app.StudentId = @StudentId";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@StudentId", studentId);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var accommodation = ReadAccommodation(reader);
                results.Add(accommodation);
            }

            return results;
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

        public async Task<int> AddAsync(Accommodation accommodation)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = @"
            INSERT INTO Accommodation 
            (Title, Description, Address, PostCode, City, Country, MonthlyRent, Size, MaxOccupants, IsAvailable, LandlordId, AccommodationTypeId, UniversityId)
            OUTPUT INSERTED.AccommodationId
            VALUES 
            (@Title, @Description, @Address, @PostCode, @City, @Country, @MonthlyRent, @Size, @MaxOccupants, @IsAvailable, @LandlordId, @AccommodationTypeId, @UniversityId)";


            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Title", accommodation.Title);
            cmd.Parameters.AddWithValue("@Description", accommodation.Description);
            cmd.Parameters.AddWithValue("@Address", accommodation.Address);
            cmd.Parameters.AddWithValue("@PostCode", accommodation.PostCode);
            cmd.Parameters.AddWithValue("@City", accommodation.City);
            cmd.Parameters.AddWithValue("@Country", accommodation.Country);
            cmd.Parameters.AddWithValue("@MonthlyRent", accommodation.MonthlyRent);
            cmd.Parameters.AddWithValue("@Size", accommodation.Size);
            cmd.Parameters.AddWithValue("@MaxOccupants", accommodation.MaxOccupants);
            cmd.Parameters.AddWithValue("@IsAvailable", accommodation.IsAvailable);
            cmd.Parameters.AddWithValue("@LandlordId", accommodation.LandlordId);
            cmd.Parameters.AddWithValue("@AccommodationTypeId", accommodation.AccommodationTypeId);
            cmd.Parameters.AddWithValue("@UniversityId", accommodation.UniversityId);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }



        public async Task UpdateAsync(Accommodation accommodation)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = @"
                UPDATE Accommodation
                SET Title = @Title, Description = @Description,Address = @Address, PostCode = @PostCode, City = @City, Country = @Country, MonthlyRent = @MonthlyRent, Size = @Size,
                    MaxOccupants = @MaxOccupants, IsAvailable = @IsAvailable,
                    AccommodationTypeId = @AccommodationTypeId, UniversityId = @UniversityId
                WHERE AccommodationId = @AccommodationId";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Title", accommodation.Title);
            cmd.Parameters.AddWithValue("@Description", accommodation.Description);
            cmd.Parameters.AddWithValue("@Address", accommodation.Address);
            cmd.Parameters.AddWithValue("@PostCode", accommodation.PostCode);
            cmd.Parameters.AddWithValue("@City", accommodation.City);
            cmd.Parameters.AddWithValue("@Country", accommodation.Country);
            cmd.Parameters.AddWithValue("@MonthlyRent", accommodation.MonthlyRent);
            cmd.Parameters.AddWithValue("@Size", accommodation.Size);
            cmd.Parameters.AddWithValue("@MaxOccupants", accommodation.MaxOccupants);
            cmd.Parameters.AddWithValue("@IsAvailable", accommodation.IsAvailable);
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

        public async Task<IEnumerable<Accommodation>> GetFeaturedAccommodationsAsync()
        {
            var accommodations = new List<Accommodation>();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "SELECT TOP 3 * FROM Accommodation WHERE IsAvailable = 1 ORDER BY AccommodationId DESC";
            using var cmd = new SqlCommand(query, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                accommodations.Add(ReadAccommodation(reader));
            }

            return accommodations;
        }

        public async Task UpdateAmenitiesAsync(int accommodationId, IEnumerable<int> amenityIds)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var transaction = conn.BeginTransaction();

            try
            {
                // 1. Delete all current amenities for this accommodation
                var deleteCmd = new SqlCommand("DELETE FROM AccommodationAmenity WHERE AccommodationId = @AccommodationId", conn, transaction);
                deleteCmd.Parameters.AddWithValue("@AccommodationId", accommodationId);
                await deleteCmd.ExecuteNonQueryAsync();

                // 2. Re-insert new ones
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




        private Accommodation ReadAccommodation(SqlDataReader reader)
        {
            return new Accommodation
            {
                AccommodationId = reader.GetInt32(reader.GetOrdinal("AccommodationId")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Address = reader.GetString(reader.GetOrdinal("Address")),
                PostCode = reader.GetString(reader.GetOrdinal("PostCode")),
                City = reader.GetString(reader.GetOrdinal("City")),
                Country = reader.GetString(reader.GetOrdinal("Country")),
                Description = reader.GetString(reader.GetOrdinal("Description")),
                MonthlyRent = reader.GetDecimal(reader.GetOrdinal("MonthlyRent")),
                Size = reader.GetDecimal(reader.GetOrdinal("Size")),
                MaxOccupants = reader.GetInt32(reader.GetOrdinal("MaxOccupants")),
                IsAvailable = reader.GetBoolean(reader.GetOrdinal("IsAvailable")),
                LandlordId = reader.GetInt32(reader.GetOrdinal("LandlordId")),
                AccommodationTypeId = reader.GetInt32(reader.GetOrdinal("AccommodationTypeId")),
                UniversityId = reader.GetInt32(reader.GetOrdinal("UniversityId")),
                AvailableFrom = reader.GetDateTime(reader.GetOrdinal("AvailableFrom")) 
            };
        }


        public async Task AddAmenitiesAsync(int accommodationId, IEnumerable<int> amenityIds)
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


        public async Task AddImagesAsync(IEnumerable<AccommodationImage> images)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            foreach (var image in images)
            {
                var cmd = new SqlCommand(@"
            INSERT INTO AccommodationImage (AccommodationId, ImageUrl, Description, UploadedAt)
            VALUES (@AccommodationId, @ImageUrl, @Description, @UploadedAt)", conn);

                cmd.Parameters.AddWithValue("@AccommodationId", image.AccommodationId);
                cmd.Parameters.AddWithValue("@ImageUrl", image.ImageUrl);
                cmd.Parameters.AddWithValue("@Description", image.Description ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@UploadedAt", image.UploadedAt);

                await cmd.ExecuteNonQueryAsync();
            }
        }



    }
}
