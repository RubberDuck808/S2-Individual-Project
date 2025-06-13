using AutoMapper;
using BLL.DTOs.Shared;
using DAL.Interfaces;
using Domain.Models;

namespace BLL.Services;
public class AmenityService : IAmenityService
{
    private readonly IAmenityRepository _repo;
    private readonly IMapper _mapper;

    public AmenityService(IAmenityRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<List<AmenityDto>> GetAllAsync()
    {
        var amenities = await _repo.GetAllAsync();
        return _mapper.Map<List<AmenityDto>>(amenities);
    }

    public async Task<List<int>> GetIdsByAccommodationIdAsync(int accommodationId)
    {
        var amenities = await _repo.GetByAccommodationIdAsync(accommodationId);
        return amenities.Select(a => a.AmenityId).ToList();
    }
}
