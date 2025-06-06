using DAL.Models;

public interface IAmenityService
{
    Task<List<Amenity>> GetAllAsync();
}