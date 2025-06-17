using BLL.DTOs.Shared;

public interface IAmenityService
{
    Task<List<AmenityDto>> GetAllAsync();
    Task UpdateAsync(int accommodationId, IEnumerable<int> amenityIds);

    Task<List<int>> GetIdsByAccommodationIdAsync(int accommodationId);
    Task<List<string>> GetNamesByAccommodationIdAsync(int accommodationId);
    Task AddAsync(int accommodationId, IEnumerable<int> amenityIds);
}
