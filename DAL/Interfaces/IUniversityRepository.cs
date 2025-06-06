using DAL.Models;

namespace DAL.Interfaces
{
    public interface IUniversityRepository
    {
        Task<int?> GetUniversityIdByEmailDomainAsync(string domain);
        Task<List<University>> GetAllAsync(); // ← Add this
    }
}
