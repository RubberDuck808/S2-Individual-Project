using BLL.DTOs.Accommodation;
using BLL.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace BLL.Services
{
    public class AccommodationAssemblerService : IAccommodationAssemblerService
    {
        private readonly IAmenityService _amenityService;
        private readonly IAccommodationImageService _imageService;
        private readonly IAccommodationTypeService _typeService;
        private readonly IUniversityService _universityService;
        private readonly ILogger<AccommodationAssemblerService> _logger;

        public AccommodationAssemblerService(
            IAmenityService amenityService,
            IAccommodationImageService imageService,
            IAccommodationTypeService typeService,
            IUniversityService universityService,
            ILogger<AccommodationAssemblerService> logger)
        {
            _amenityService = amenityService;
            _imageService = imageService;
            _typeService = typeService;
            _universityService = universityService;
            _logger = logger;
        }


        public async Task<AccommodationDto> ToDtoAsync(Accommodation entity)
        {
            _logger.LogInformation("Assembling DTO for accommodation ID: {Id}", entity.AccommodationId);

            var amenities = await _amenityService.GetNamesByAccommodationIdAsync(entity.AccommodationId);
            var images = await _imageService.GetUrlsByAccommodationIdAsync(entity.AccommodationId);
            var universityName = await _universityService.GetNameByIdAsync(entity.UniversityId);
            var typeName = await _typeService.GetNameByIdAsync(entity.AccommodationTypeId);

            return new AccommodationDto
            {
                AccommodationId = entity.AccommodationId,
                Title = entity.Title,
                Description = entity.Description,
                Address = entity.Address,
                PostCode = entity.PostCode,
                City = entity.City,
                Country = entity.Country,
                MonthlyRent = entity.MonthlyRent,
                IsAvailable = entity.IsAvailable,
                MaxOccupants = entity.MaxOccupants,
                Size = (int)entity.Size,
                AvailableFrom = entity.AvailableFrom,
                Latitude = entity.Latitude,           
                Longitude = entity.Longitude,         
                AmenityNames = amenities,
                ImageUrls = images,
                UniversityName = universityName,
                AccommodationType = typeName,
                LandlordName = "",
                UniversityId = entity.UniversityId,
                AccommodationTypeId = entity.AccommodationTypeId
            };
        }

    }
}
