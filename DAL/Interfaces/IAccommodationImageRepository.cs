using DAL.Models;

public interface IAccommodationImageRepository
{
    Task<List<AccommodationImage>> GetByAccommodationIdAsync(int accommodationId);
}
