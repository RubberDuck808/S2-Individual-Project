using Microsoft.Data.SqlClient;
using DAL.Interfaces;

namespace DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<string> CreateUserAsync(string email, string passwordHash, string phoneNumber, string firstName, string lastName)
        {
            var userId = Guid.NewGuid().ToString();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            var cmd = new SqlCommand(@"
                INSERT INTO AspNetUsers 
                (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed,
                 PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumber, PhoneNumberConfirmed,
                 TwoFactorEnabled, LockoutEnabled, AccessFailedCount,
                 FirstName, LastName)
                VALUES
                (@Id, @UserName, @NormalizedUserName, @Email, @NormalizedEmail, 0,
                 @PasswordHash, NEWID(), NEWID(), @PhoneNumber, 0, 0, 0, 0,
                 @FirstName, @LastName)", conn);

            cmd.Parameters.AddWithValue("@Id", userId);
            cmd.Parameters.AddWithValue("@UserName", email);
            cmd.Parameters.AddWithValue("@NormalizedUserName", email.ToUpper());
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@NormalizedEmail", email.ToUpper());
            cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
            cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
            cmd.Parameters.AddWithValue("@FirstName", firstName);
            cmd.Parameters.AddWithValue("@LastName", lastName);

            await cmd.ExecuteNonQueryAsync();
            return userId;
        }

        public async Task AssignRoleAsync(string userId, string roleName)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            // Get roleId
            var getRoleCmd = new SqlCommand(
                "SELECT Id FROM AspNetRoles WHERE NormalizedName = @RoleName", conn);
            getRoleCmd.Parameters.AddWithValue("@RoleName", roleName.ToUpper());

            var roleIdObj = await getRoleCmd.ExecuteScalarAsync();
            if (roleIdObj == null)
                throw new Exception($"Role '{roleName}' not found.");

            var roleId = (string)roleIdObj;

            // Insert into AspNetUserRoles
            var assignCmd = new SqlCommand(
                "INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@UserId, @RoleId)", conn);
            assignCmd.Parameters.AddWithValue("@UserId", userId);
            assignCmd.Parameters.AddWithValue("@RoleId", roleId);

            await assignCmd.ExecuteNonQueryAsync();
        }
    }
}
