using BLL.DTOs.Accommodation;



public interface IAccommodationTypeService
{
    Task<List<AccommodationTypeDto>> GetAllAsync();
    Task<AccommodationTypeDto?> GetByIdAsync(int id);
    Task<string> GetNameByIdAsync(int id);
}
