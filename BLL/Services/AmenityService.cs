using AutoMapper;
using BLL.DTOs.Shared;
using DAL.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace BLL.Services
{
    public class AmenityService : IAmenityService
    {
        private readonly IAmenityRepository _repo;
        private readonly IMapper _mapper;
        private readonly ILogger<AmenityService> _logger;

        public AmenityService(IAmenityRepository repo, IMapper mapper, ILogger<AmenityService> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<AmenityDto>> GetAllAsync()
        {
            _logger.LogInformation("Fetching all amenities");

            var amenities = await _repo.GetAllAsync();
            _logger.LogInformation("Retrieved {Count} amenities", amenities.Count);

            return _mapper.Map<List<AmenityDto>>(amenities);
        }

        public async Task<List<int>> GetIdsByAccommodationIdAsync(int accommodationId)
        {
            _logger.LogInformation("Fetching amenity IDs for accommodation ID: {AccommodationId}", accommodationId);

            var amenities = await _repo.GetByAccommodationIdAsync(accommodationId);
            var ids = amenities.Select(a => a.AmenityId).ToList();

            _logger.LogInformation("Retrieved {Count} amenity IDs for accommodation ID: {AccommodationId}", ids.Count, accommodationId);

            return ids;
        }

        public async Task<List<string>> GetNamesByAccommodationIdAsync(int accommodationId)
        {
            _logger.LogInformation("Fetching amenity names for accommodation ID: {AccommodationId}", accommodationId);

            var amenities = await _repo.GetByAccommodationIdAsync(accommodationId);
            var names = amenities.Select(a => a.Name).ToList();

            _logger.LogInformation("Retrieved {Count} amenity names for accommodation ID: {AccommodationId}", names.Count, accommodationId);

            return names;
        }

        public async Task AddAsync(int accommodationId, IEnumerable<int> amenityIds)
        {
            await _repo.AddAsync(accommodationId, amenityIds);
        }

        public async Task UpdateAsync(int accommodationId, IEnumerable<int> amenityIds)
        {
            await _repo.UpdateAsync(accommodationId, amenityIds);
        }



    }
}
