using DAL.Interfaces;
using DAL.Models;

public class AccommodationTypeService : IAccommodationTypeService
{
    private readonly IAccommodationTypeRepository _repo;
    public AccommodationTypeService(IAccommodationTypeRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<AccommodationType>> GetAllAsync()
    {
        return await _repo.GetAllAsync();
    }
}