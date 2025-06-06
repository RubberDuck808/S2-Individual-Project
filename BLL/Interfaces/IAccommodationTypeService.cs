using DAL.Models;

public interface IAccommodationTypeService
{
    Task<List<AccommodationType>> GetAllAsync();
}