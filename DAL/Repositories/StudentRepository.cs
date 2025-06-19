using Microsoft.Data.SqlClient;
using Domain.Models;
using DAL.Interfaces;

namespace DAL.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly string _connectionString;

        public StudentRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<Student?> GetByIdAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "SELECT * FROM Student WHERE StudentId = @Id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? ReadStudent(reader) : null;
        }

        public async Task<Student?> GetByUserIdAsync(string userId)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "SELECT * FROM Student WHERE UserId = @UserId";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserId", userId);

            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? ReadStudent(reader) : null;
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
        {
            var students = new List<Student>();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "SELECT * FROM Student";
            using var cmd = new SqlCommand(query, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                students.Add(ReadStudent(reader));
            }

            return students;
        }

        public Task<IEnumerable<Student>> FindAsync(System.Linq.Expressions.Expression<Func<Student, bool>> predicate)
        {
            throw new NotSupportedException("FindAsync with expressions is not supported in ADO.NET.");
        }

        public async Task<int> AddAsync(Student student)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = @"
            INSERT INTO Student 
            (UserId, UniversityId, Email, FirstName, MiddleName, LastName, DateOfBirth, PhoneNumber, EmergencyContact, EmergencyPhone, ProfileImageUrl, IsVerified, CreatedAt, UpdatedAt)
            OUTPUT INSERTED.StudentId
            VALUES 
            (@UserId, @UniversityId, @Email, @FirstName, @MiddleName, @LastName, @DateOfBirth, @PhoneNumber, @EmergencyContact, @EmergencyPhone, @ProfileImageUrl, @IsVerified, @CreatedAt, @UpdatedAt)";

            using var cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@UserId", student.UserId);
            cmd.Parameters.AddWithValue("@UniversityId", student.UniversityId);
            cmd.Parameters.AddWithValue("@Email", student.Email);
            cmd.Parameters.AddWithValue("@FirstName", student.FirstName);
            cmd.Parameters.AddWithValue("@MiddleName", (object?)student.MiddleName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LastName", student.LastName);
            cmd.Parameters.AddWithValue("@DateOfBirth", student.DateOfBirth);
            cmd.Parameters.AddWithValue("@PhoneNumber", student.PhoneNumber);
            cmd.Parameters.AddWithValue("@EmergencyContact", student.EmergencyContact);
            cmd.Parameters.AddWithValue("@EmergencyPhone", student.EmergencyPhone);
            cmd.Parameters.AddWithValue("@ProfileImageUrl", student.ProfileImageUrl);
            cmd.Parameters.AddWithValue("@IsVerified", student.IsVerified);
            cmd.Parameters.AddWithValue("@CreatedAt", student.CreatedAt);
            cmd.Parameters.AddWithValue("@UpdatedAt", student.UpdatedAt);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }


        public async Task UpdateAsync(Student student)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = @"
                UPDATE Student
                SET UniversityId = @UniversityId,
                    Email = @Email,
                    FirstName = @FirstName,
                    MiddleName = @MiddleName,
                    LastName = @LastName,
                    DateOfBirth = @DateOfBirth,
                    PhoneNumber = @PhoneNumber,
                    EmergencyContact = @EmergencyContact,
                    EmergencyPhone = @EmergencyPhone,
                    ProfileImageUrl = @ProfileImageUrl,
                    IsVerified = @IsVerified,
                    UpdatedAt = @UpdatedAt
                WHERE StudentId = @StudentId";

            using var cmd = new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@UniversityId", student.UniversityId);
            cmd.Parameters.AddWithValue("@Email", student.Email);
            cmd.Parameters.AddWithValue("@FirstName", student.FirstName);
            cmd.Parameters.AddWithValue("@MiddleName", (object?)student.MiddleName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LastName", student.LastName);
            cmd.Parameters.AddWithValue("@DateOfBirth", student.DateOfBirth);
            cmd.Parameters.AddWithValue("@PhoneNumber", student.PhoneNumber);
            cmd.Parameters.AddWithValue("@EmergencyContact", student.EmergencyContact);
            cmd.Parameters.AddWithValue("@EmergencyPhone", student.EmergencyPhone);
            cmd.Parameters.AddWithValue("@ProfileImageUrl", student.ProfileImageUrl);
            cmd.Parameters.AddWithValue("@IsVerified", student.IsVerified);
            cmd.Parameters.AddWithValue("@UpdatedAt", student.UpdatedAt);
            cmd.Parameters.AddWithValue("@StudentId", student.StudentId);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "DELETE FROM Student WHERE StudentId = @Id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "SELECT 1 FROM Student WHERE StudentId = @Id";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Id", id);

            var result = await cmd.ExecuteScalarAsync();
            return result != null;
        }

        public async Task<int> GetCountAsync()
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var query = "SELECT COUNT(*) FROM Student";
            using var cmd = new SqlCommand(query, conn);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        private Student ReadStudent(SqlDataReader reader)
        {
            return new Student
            {
                StudentId = reader.GetInt32(reader.GetOrdinal("StudentId")),
                UserId = reader.GetString(reader.GetOrdinal("UserId")),
                UniversityId = reader.GetInt32(reader.GetOrdinal("UniversityId")),
                Email = reader.GetString(reader.GetOrdinal("Email")),
                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                MiddleName = reader.IsDBNull(reader.GetOrdinal("MiddleName")) ? null : reader.GetString(reader.GetOrdinal("MiddleName")),
                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                DateOfBirth = reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                EmergencyContact = reader.GetString(reader.GetOrdinal("EmergencyContact")),
                EmergencyPhone = reader.GetString(reader.GetOrdinal("EmergencyPhone")),
                ProfileImageUrl = reader.GetString(reader.GetOrdinal("ProfileImageUrl")),
                IsVerified = reader.GetBoolean(reader.GetOrdinal("IsVerified")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("UpdatedAt"))
            };
        }
    }
}
