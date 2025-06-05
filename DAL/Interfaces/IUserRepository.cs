using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IUserRepository
    {
        Task<string> CreateUserAsync(string email, string passwordHash, string phoneNumber, string firstName, string lastName);
        Task AssignRoleAsync(string userId, string roleName);
        Task<(string UserId, string PasswordHash)> GetUserAuthDataAsync(string email);
        Task<List<string>> GetRolesAsync(string userId);

    }
}
