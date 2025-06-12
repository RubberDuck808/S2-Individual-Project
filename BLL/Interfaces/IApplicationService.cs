using BLL.DTOs.Application;

namespace BLL.Interfaces
{
    public interface IApplicationService
    {
        Task<IEnumerable<ApplicationDto>> GetByStudentAsync(int studentId);
        Task<IEnumerable<ApplicationDto>> GetByLandlordAsync(int landlordId);
        Task<ApplicationDto> GetByIdAsync(int id);
        Task<int> CreateAsync(ApplicationCreateDto dto);
        Task UpdateStatusAsync(ApplicationUpdateDto dto);
        Task<bool> ExistsAsync(int studentId, int accommodationId);
        Task<List<ApplicationDto>> GetByAccommodationIdAsync(int accommodationId);
        Task SelectApplicantAsync(int applicationId, int accommodationId);


    }
}
