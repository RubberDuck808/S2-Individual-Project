using BLL.DTOs.Accommodation;
using DAL.Models;

namespace BLL.Interfaces
{
    public interface IAccommodationService
    {
        Task<IEnumerable<AccommodationDto>> GetAllAsync();
        Task<IEnumerable<AccommodationDto>> GetIndexAsync();
        Task<AccommodationDto> GetByIdAsync(int id);
        Task<int> CreateAsync(AccommodationCreateDto dto);
        Task UpdateAsync(AccommodationUpdateDto dto);
        Task DeleteAsync(int id);
        Task AddAmenitiesAsync(int accommodationId, IEnumerable<int> amenityIds);
        Task AddImagesAsync(IEnumerable<AccommodationImage> images);

        Task<int> CreateAccommodationWithAmenitiesAsync(AccommodationCreateDto dto, IEnumerable<int> amenityIds);

        Task<IEnumerable<LandlordAccommodationDto>> GetByLandlordUserIdAsync(string landlordUserId);






    }
}
