using AutoMapper;
using BLL.DTOs.Accommodation;
using BLL.Exceptions;
using BLL.Interfaces;
using DAL.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;

public class AccommodationTypeService : IAccommodationTypeService
{
    private readonly IAccommodationTypeRepository _repo;
    private readonly IMapper _mapper;
    private readonly ILogger<AccommodationTypeService> _logger;

    public AccommodationTypeService(
        IAccommodationTypeRepository repo,
        IMapper mapper,
        ILogger<AccommodationTypeService> logger)
    {
        _repo = repo;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<AccommodationTypeDto>> GetAllAsync()
    {
        _logger.LogInformation("Fetching all accommodation types");

        var types = await _repo.GetAllAsync();

        _logger.LogInformation("Retrieved {Count} accommodation types", types.Count);
        return _mapper.Map<List<AccommodationTypeDto>>(types);
    }

    public async Task<AccommodationTypeDto> GetByIdAsync(int id)
    {
        _logger.LogInformation("Fetching accommodation type with ID: {Id}", id);

        var type = await _repo.GetByIdAsync(id);
        if (type == null)
        {
            _logger.LogWarning("Accommodation type with ID {Id} not found", id);
            throw new NotFoundException(string.Format(ErrorMessages.AccommodationNotFound, id));

        }

        _logger.LogInformation("Accommodation type {Id} found: {Name}", id, type.Name);
        return _mapper.Map<AccommodationTypeDto>(type);
    }

    public async Task<string> GetNameByIdAsync(int id)
    {
        _logger.LogInformation("Fetching accommodation type name with ID: {Id}", id);

        var type = await _repo.GetByIdAsync(id);
        return type?.Name ?? "";
    }

}
