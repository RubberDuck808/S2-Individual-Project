using Domain.Models;

public interface IAccommodationImageRepository
{
    Task<List<AccommodationImage>> GetByAccommodationIdAsync(int accommodationId);

    Task AddImagesAsync(IEnumerable<AccommodationImage> images);
}
