using BLL.Interfaces;
using DAL.Interfaces;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace BLL.Services
{
    public class AccommodationImageService : IAccommodationImageService
    {
        private readonly IAccommodationImageRepository _imageRepo;
        private readonly ILogger<AccommodationImageService> _logger;

        public AccommodationImageService(IAccommodationImageRepository imageRepo, ILogger<AccommodationImageService> logger)
        {
            _imageRepo = imageRepo;
            _logger = logger;
        }

        public async Task<List<AccommodationImage>> GetByAccommodationIdAsync(int accommodationId)
        {
            _logger.LogInformation("Fetching images for accommodation ID: {Id}", accommodationId);
            return await _imageRepo.GetByAccommodationIdAsync(accommodationId);
        }

        public async Task AddImagesAsync(IEnumerable<AccommodationImage> images)
        {
            _logger.LogInformation("Adding {Count} images to accommodation ID: {Id}",
                images.Count(),
                images.FirstOrDefault()?.AccommodationId ?? 0);

            await _imageRepo.AddImagesAsync(images);
        }

        public async Task<List<string>> GetUrlsByAccommodationIdAsync(int accommodationId)
        {
            _logger.LogInformation("Fetching image URLs for accommodation ID: {Id}", accommodationId);
            var images = await _imageRepo.GetByAccommodationIdAsync(accommodationId);
            return images.Select(i => i.ImageUrl).ToList();
        }

        public async Task DeleteByAccommodationIdAsync(int accommodationId)
        {
            _logger.LogInformation("Deleting all images for accommodation ID {Id}", accommodationId);
            await _imageRepo.DeleteByAccommodationIdAsync(accommodationId);
        }
    }
}
