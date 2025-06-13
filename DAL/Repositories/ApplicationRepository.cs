using Microsoft.Data.SqlClient;
using DAL.Interfaces;
using Domain.Models;

namespace DAL.Repositories
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly string _connectionString;

        public ApplicationRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Application>> GetByStudentAsync(int studentId)
        {
            var applications = new List<Application>();

            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand("SELECT * FROM Application WHERE StudentId = @studentId", connection);
            command.Parameters.AddWithValue("@studentId", studentId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                applications.Add(new Application
                {
                    ApplicationId = (int)reader["ApplicationId"],
                    StudentId = (int)reader["StudentId"],
                    AccommodationId = (int)reader["AccommodationId"],
                    StatusId = (int)reader["StatusId"],
                    ApplicationDate = (DateTime)reader["ApplicationDate"]
                });
            }

            return applications;
        }

        public async Task<IEnumerable<Application>> GetByLandlordIdAsync(int landlordId)
        {
            var applications = new List<Application>();

            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand(@"
                SELECT a.* 
                FROM Application a
                INNER JOIN Accommodation acc ON a.AccommodationId = acc.AccommodationId
                WHERE acc.LandlordId = @landlordId", connection);
            command.Parameters.AddWithValue("@landlordId", landlordId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                applications.Add(new Application
                {
                    ApplicationId = (int)reader["ApplicationId"],
                    StudentId = (int)reader["StudentId"],
                    AccommodationId = (int)reader["AccommodationId"],
                    StatusId = (int)reader["StatusId"],
                    ApplicationDate = (DateTime)reader["ApplicationDate"]
                });
            }

            return applications;
        }

        public async Task<Application?> GetByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand("SELECT * FROM Application WHERE ApplicationId = @id", connection);
            command.Parameters.AddWithValue("@id", id);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Application
                {
                    ApplicationId = (int)reader["ApplicationId"],
                    StudentId = (int)reader["StudentId"],
                    AccommodationId = (int)reader["AccommodationId"],
                    StatusId = (int)reader["StatusId"],
                    ApplicationDate = (DateTime)reader["ApplicationDate"]
                };
            }

            return null;
        }

        public async Task AddAsync(Application app)
        {
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand(@"
                INSERT INTO Application (StudentId, AccommodationId, StatusId, ApplicationDate)
                VALUES (@studentId, @accommodationId, @statusId, @applicationDate);
                SELECT CAST(SCOPE_IDENTITY() AS INT);", connection);

            command.Parameters.AddWithValue("@studentId", app.StudentId);
            command.Parameters.AddWithValue("@accommodationId", app.AccommodationId);
            command.Parameters.AddWithValue("@statusId", app.StatusId);
            command.Parameters.AddWithValue("@applicationDate", app.ApplicationDate);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            app.ApplicationId = Convert.ToInt32(result);
        }

        public async Task UpdateAsync(Application app)
        {
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand(@"
                UPDATE Application
                SET StatusId = @statusId
                WHERE ApplicationId = @applicationId", connection);

            command.Parameters.AddWithValue("@statusId", app.StatusId);
            command.Parameters.AddWithValue("@applicationId", app.ApplicationId);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<bool> ExistsAsync(int studentId, int accommodationId)
        {
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand(@"
                SELECT COUNT(1)
                FROM Application
                WHERE StudentId = @studentId AND AccommodationId = @accommodationId", connection);

            command.Parameters.AddWithValue("@studentId", studentId);
            command.Parameters.AddWithValue("@accommodationId", accommodationId);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        public async Task<IEnumerable<Application>> GetAllAsync()
        {
            var applications = new List<Application>();
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand("SELECT * FROM Application", connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                applications.Add(new Application
                {
                    ApplicationId = (int)reader["ApplicationId"],
                    StudentId = (int)reader["StudentId"],
                    AccommodationId = (int)reader["AccommodationId"],
                    StatusId = (int)reader["StatusId"],
                    ApplicationDate = (DateTime)reader["ApplicationDate"]
                });
            }

            return applications;
        }

        public async Task<int> CreateAsync(Application app)
        {
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand(@"
                INSERT INTO Application (StudentId, AccommodationId, StatusId, ApplicationDate)
                VALUES (@studentId, @accommodationId, @statusId, @applicationDate);
                SELECT CAST(SCOPE_IDENTITY() AS INT);", connection);

            command.Parameters.AddWithValue("@studentId", app.StudentId);
            command.Parameters.AddWithValue("@accommodationId", app.AccommodationId);
            command.Parameters.AddWithValue("@statusId", app.StatusId);
            command.Parameters.AddWithValue("@applicationDate", app.ApplicationDate);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand("SELECT COUNT(1) FROM Application WHERE ApplicationId = @id", connection);
            command.Parameters.AddWithValue("@id", id);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand("DELETE FROM Application WHERE ApplicationId = @id", connection);
            command.Parameters.AddWithValue("@id", id);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<string?> GetStatusNameByStudentAndAccommodationIdAsync(int studentId, int accommodationId)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = @"
            SELECT s.Name
            FROM Application a
            INNER JOIN Status s ON a.StatusId = s.StatusId
            WHERE a.StudentId = @StudentId AND a.AccommodationId = @AccommodationId";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@StudentId", studentId);
            cmd.Parameters.AddWithValue("@AccommodationId", accommodationId);

            var result = await cmd.ExecuteScalarAsync();
            return result as string;
        }

        public async Task<List<Application>> GetByAccommodationIdAsync(int accommodationId)
        {
            var applications = new List<Application>();

            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand("SELECT * FROM Application WHERE AccommodationId = @accId", connection);
            command.Parameters.AddWithValue("@accId", accommodationId);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                applications.Add(new Application
                {
                    ApplicationId = (int)reader["ApplicationId"],
                    StudentId = (int)reader["StudentId"],
                    AccommodationId = (int)reader["AccommodationId"],
                    StatusId = (int)reader["StatusId"],
                    ApplicationDate = (DateTime)reader["ApplicationDate"]
                });
            }

            return applications;
        }

        public async Task MarkAsSelectedAsync(int selectedAppId)
        {
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand(@"
            UPDATE Application
            SET StatusId = 2 -- Assuming 2 = Selected
            WHERE ApplicationId = @id", connection);

            command.Parameters.AddWithValue("@id", selectedAppId);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }


        public async Task RejectOthersAsync(int accommodationId, int selectedAppId)
        {
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand(@"
        UPDATE Application
        SET StatusId = 3 -- Assuming 3 = Rejected
        WHERE AccommodationId = @accId AND ApplicationId != @selectedId", connection);

            command.Parameters.AddWithValue("@accId", accommodationId);
            command.Parameters.AddWithValue("@selectedId", selectedAppId);

            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }


    }
}
