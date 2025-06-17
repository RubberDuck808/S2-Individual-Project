using BLL.DTOs.Landlord;

namespace BLL.Interfaces
{
    public interface ILandlordService
    {
        Task<LandlordDto> GetLandlordAsync(int id);
        Task UpdateVerificationStatusAsync(int landlordId, LandlordVerificationDto dto);
        Task<int> CreateAsync(LandlordRegistrationDto dto);

        Task<IEnumerable<LandlordDto>> GetLandlordsByUniversityAsync(int universityId);
        Task<IEnumerable<LandlordDto>> GetAllVerifiedLandlordsAsync();
        Task UpdateLandlordProfileAsync(int landlordId, LandlordUpdateDto updateDto);
        Task<IEnumerable<LandlordDto>> GetAllAsync();
        Task<LandlordAdminDto> GetLandlordForAdminAsync(int id);
        Task<LandlordBasicDto> GetPublicLandlordAsync(int id);
        Task<LandlordDto?> GetByUserIdAsync(string userId);


    }
}
