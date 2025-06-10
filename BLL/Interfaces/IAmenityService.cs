using BLL.DTOs.Shared;

public interface IAmenityService
{
    Task<List<AmenityDto>> GetAllAsync();
    Task<List<int>> GetIdsByAccommodationIdAsync(int accommodationId);
}
