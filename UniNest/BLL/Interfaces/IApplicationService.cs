using UniNest.BLL.DTOs.Application;

namespace UniNest.BLL.Interfaces
{
    public interface IApplicationService
    {
        Task<IEnumerable<ApplicationDto>> GetByStudentAsync(int studentId);
        Task<IEnumerable<ApplicationDto>> GetByLandlordAsync(int landlordId);
        Task<ApplicationDto> GetByIdAsync(int id);
        Task<int> CreateAsync(ApplicationCreateDto dto);
        Task UpdateStatusAsync(ApplicationUpdateDto dto);
    }
}
