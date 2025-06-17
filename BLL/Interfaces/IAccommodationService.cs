using BLL.DTOs.Accommodation;
using BLL.DTOs.Application;
using Domain.Models;

namespace BLL.Interfaces
{
    public interface IAccommodationService
    {
        Task<IEnumerable<AccommodationDto>> GetAllAsync();
        Task<IEnumerable<AccommodationDto>> GetIndexAsync();
        Task<AccommodationDto> GetByIdAsync(int id);
        Task UpdateAsync(AccommodationUpdateDto dto);
        Task DeleteAsync(int id);
        Task<int> CreateAsync(AccommodationCreateDto dto, IEnumerable<int> amenityIds);
        Task<int> UpdateWithAmenitiesAsync(AccommodationUpdateDto dto, IEnumerable<int> amenityIds);
        Task<IEnumerable<LandlordAccommodationDto>> GetByLandlordUserIdAsync(string landlordUserId);
        Task<IEnumerable<AppliedAccommodationDto>> GetApplicationByStudentUserIdAsync(string studentUserId);
        Task<IEnumerable<AccommodationBookingDto>> GetBookingsByStudentUserIdAsync(string studentUserId);


    }

}
