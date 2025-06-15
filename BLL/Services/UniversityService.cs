using AutoMapper;
using BLL.DTOs.Shared;
using BLL.Exceptions;
using BLL.Interfaces;
using Domain.Models;
using DAL.Interfaces;
using Microsoft.Extensions.Logging;

namespace BLL.Services
{
    public class UniversityService : IUniversityService
    {
        private readonly IUniversityRepository _universityRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<UniversityService> _logger;

        public UniversityService(IUniversityRepository universityRepo, IMapper mapper, ILogger<UniversityService> logger)
        {
            _universityRepo = universityRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<UniversityDto>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all universities");

            var universities = await _universityRepo.GetAllAsync();
            _logger.LogInformation("Retrieved {Count} universities", universities.Count);

            return _mapper.Map<List<UniversityDto>>(universities);
        }

        public async Task<UniversityDto> GetByIdAsync(int id)
        {
            _logger.LogInformation("Fetching university with ID: {Id}", id);

            var university = await _universityRepo.GetByIdAsync(id);
            if (university == null)
            {
                _logger.LogWarning("University with ID {Id} not found", id);
                throw new NotFoundException($"University with ID {id} not found");
            }

            _logger.LogInformation("University with ID {Id} retrieved successfully", id);
            return _mapper.Map<UniversityDto>(university);
        }
    }
}
