using DAL.Models;

public interface IAccommodationTypeService
{
    Task<List<AccommodationType>> GetAllAsync();

    Task<AccommodationType?> GetByIdAsync(int id);

}