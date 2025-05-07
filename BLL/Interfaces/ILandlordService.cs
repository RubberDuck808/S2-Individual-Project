using BLL.DTOs.Landlord;

namespace BLL.Interfaces
{
    public interface ILandlordService
    {
        Task<LandlordDto> GetLandlordAsync(int id);
        Task VerifyLandlordAsync(int landlordId);
        Task<IEnumerable<LandlordDto>> GetLandlordsByUniversityAsync(int universityId);
        Task<IEnumerable<LandlordDto>> GetAllVerifiedLandlordsAsync();
        Task UpdateLandlordProfileAsync(int landlordId, LandlordUpdateDto updateDto);
        Task<IEnumerable<LandlordDto>> GetAllAsync();

    }
}
