using DAL.Models;

namespace DAL.Interfaces
{
    public interface IApplicationRepository
    {
        Task<IEnumerable<Application>> GetAllAsync();
        Task<Application?> GetByIdAsync(int id);
        Task<IEnumerable<Application>> GetByStudentAsync(int studentId);
        Task<IEnumerable<Application>> GetByLandlordIdAsync(int landlordId);
        Task<int> CreateAsync(Application application);
        Task UpdateAsync(Application application);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> ExistsAsync(int studentId, int accommodationId);

        Task<string?> GetStatusNameByStudentAndAccommodationIdAsync(int studentId, int accommodationId);

        Task<List<Application>> GetByAccommodationIdAsync(int accommodationId);
        Task MarkAsSelectedAsync(int selectedAppId);
        Task RejectOthersAsync(int accommodationId, int selectedAppId);


    }
}
