using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface ILandlordRepository
    {
        Task<Landlord?> GetByIdAsync(int id);
        Task<IEnumerable<Landlord>> GetAllAsync();
        Task AddAsync(Landlord landlord);
        Task UpdateAsync(Landlord landlord);
        Task DeleteAsync(int id);

        // Specialized Queries
        Task<Landlord> GetByIdWithPropertiesAsync(int id);
        Task<IEnumerable<Landlord>> GetByUniversityAsync(int universityId);
        Task<IEnumerable<Landlord>> GetActiveLandlordsAsync();
        Task<bool> ExistsAsync(int id);

        // User-related operations
        Task<Landlord?> GetByUserIdAsync(string userId);
        Task<int> GetCountAsync();


    }
}