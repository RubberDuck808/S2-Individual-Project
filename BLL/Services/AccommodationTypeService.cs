using AutoMapper;
using BLL.DTOs.Accommodation;
using BLL.Exceptions;
using BLL.Interfaces;
using DAL.Interfaces;
using DAL.Models;

public class AccommodationTypeService : IAccommodationTypeService
{
    private readonly IAccommodationTypeRepository _repo;
    private readonly IMapper _mapper;

    public AccommodationTypeService(IAccommodationTypeRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<List<AccommodationTypeDto>> GetAllAsync()
    {
        var types = await _repo.GetAllAsync();
        return _mapper.Map<List<AccommodationTypeDto>>(types);
    }

    public async Task<AccommodationTypeDto> GetByIdAsync(int id)
    {
        var type = await _repo.GetByIdAsync(id);
        if (type == null)
            throw new NotFoundException($"Accommodation type {id} not found");

        return _mapper.Map<AccommodationTypeDto>(type);
    }
}
