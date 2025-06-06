using DAL.Interfaces;
using DAL.Models;

public class AmenityService : IAmenityService
{
    private readonly IAmenityRepository _repo;
    public AmenityService(IAmenityRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<Amenity>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }
}