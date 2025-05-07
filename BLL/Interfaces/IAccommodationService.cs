using BLL.DTOs.Accommodation;

namespace BLL.Interfaces
{
    public interface IAccommodationService
    {
        Task<IEnumerable<AccommodationDto>> GetAllAsync();
        Task<AccommodationDto> GetByIdAsync(int id);
        Task<int> CreateAsync(AccommodationCreateDto dto);
        Task UpdateAsync(AccommodationUpdateDto dto);
        Task DeleteAsync(int id);
    }
}
