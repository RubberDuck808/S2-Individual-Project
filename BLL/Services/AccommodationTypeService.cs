using BLL.Exceptions;
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

    public async Task<AccommodationType> GetByIdAsync(int id)
    {
        var type = await _repo.GetByIdAsync(id);
        if (type == null)
            throw new NotFoundException($"Accommodation type {id} not found");

        return type;
    }

}