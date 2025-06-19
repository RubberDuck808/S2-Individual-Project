using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BLL.DTOs.Accommodation;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using UI.ViewModels;
using System.Security.Claims;
using BLL.Exceptions;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace UI.Pages.Dashboard.Landlord
{
    [Authorize(Roles = "Landlord")]
    public class EditListingModel : PageModel
    {
        private readonly ILogger<EditListingModel> _logger;
        private readonly IAccommodationService _accommodationService;
        private readonly IAmenityService _amenityService;
        private readonly IAccommodationImageService _imageService;
        private readonly IAccommodationTypeService _typeService;
        private readonly IUniversityService _universityService;
        private readonly ILandlordService _landlordService;
        private readonly IGeoLocationService _geoLocationService;
        private readonly IWebHostEnvironment _environment;

        [BindProperty]
        public AccommodationViewModel Input { get; set; } = new();

        [TempData]
        public string Message { get; set; }

        public EditListingModel(
            ILogger<EditListingModel> logger,
            IAccommodationImageService imageService,
            IAccommodationService accommodationService,
            IAmenityService amenityService,
            IAccommodationTypeService typeService,
            IUniversityService universityService,
            ILandlordService landlordService,
            IGeoLocationService geoLocationService,
            IWebHostEnvironment environment)
        {
            _logger = logger;
            _imageService = imageService;
            _accommodationService = accommodationService;
            _amenityService = amenityService;
            _typeService = typeService;
            _universityService = universityService;
            _landlordService = landlordService;
            _geoLocationService = geoLocationService;
            _environment = environment;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var accommodation = await _accommodationService.GetByIdAsync(id);
                if (accommodation == null)
                    return NotFound();

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var landlord = await _landlordService.GetByUserIdAsync(userId);

                Input = new AccommodationViewModel
                {
                    AccommodationId = accommodation.AccommodationId,
                    Title = accommodation.Title,
                    Description = accommodation.Description,
                    Address = accommodation.Address,
                    PostCode = accommodation.PostCode,
                    City = accommodation.City,
                    Country = accommodation.Country,
                    MonthlyRent = accommodation.MonthlyRent,
                    Size = accommodation.Size,
                    MaxOccupants = accommodation.MaxOccupants,
                    AccommodationTypeId = accommodation.AccommodationTypeId,
                    UniversityId = accommodation.UniversityId,
                    SelectedAmenityIds = await _amenityService.GetIdsByAccommodationIdAsync(id),
                    Amenities = await _amenityService.GetAllAsync(),
                    AccommodationTypes = (await _typeService.GetAllAsync()).ToList(),
                    Universities = (await _universityService.GetAllAsync()).ToList()
                };

                _logger.LogInformation("Edit GET requested for listing ID {Id}", id);
                return Page();
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var fullAddress = $"{Input.Address}, {Input.PostCode} {Input.City}, {Input.Country}";
            var coordinates = await _geoLocationService.GetCoordinatesFromAddressAsync(fullAddress);

            if (coordinates == null)
            {
                _logger.LogWarning("Address validation failed during edit: {Address}", fullAddress);
                ModelState.AddModelError("Input.Address", "We couldn't verify this address. Please check the street name.");
                ModelState.AddModelError("Input.PostCode", "We couldn't verify this postcode. Please check the value.");
                await LoadFormOptionsAsync();
                return Page();
            }

            var allUniversities = await _universityService.GetAllAsync();
            var closestUniversity = allUniversities
                .Where(u => u.Latitude.HasValue && u.Longitude.HasValue)
                .OrderBy(u => _geoLocationService.CalculateDistanceKm(coordinates.Value.lat, coordinates.Value.lng, u.Latitude.Value, u.Longitude.Value))
                .FirstOrDefault();

            if (closestUniversity == null)
            {
                _logger.LogWarning("No matching university found during edit based on coordinates [{Lat}, {Lng}]", coordinates.Value.lat, coordinates.Value.lng);
                ModelState.AddModelError(string.Empty, "We couldn't match this location to any university.");
                await LoadFormOptionsAsync();
                return Page();
            }

            if (!ModelState.IsValid)
            {
                await LoadFormOptionsAsync();
                return Page();
            }

            try
            {
                _logger.LogInformation("Submitting edit for accommodation ID {Id}", Input.AccommodationId);

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var landlord = await _landlordService.GetByUserIdAsync(userId);

                var dto = new AccommodationUpdateDto
                {
                    AccommodationId = Input.AccommodationId,
                    Title = Input.Title,
                    Description = Input.Description,
                    Address = Input.Address,
                    PostCode = Input.PostCode,
                    City = Input.City,
                    Country = Input.Country,
                    MonthlyRent = Input.MonthlyRent,
                    Size = Input.Size,
                    MaxOccupants = Input.MaxOccupants,
                    AccommodationTypeId = Input.AccommodationTypeId,
                    UniversityId = closestUniversity.UniversityId,
                    Latitude = coordinates.Value.lat,
                    Longitude = coordinates.Value.lng,
                    IsAvailable = true,
                    AvailableFrom = DateTime.UtcNow,
                    LandlordId = landlord.LandlordId

                };

                await _accommodationService.UpdateWithAmenitiesAsync(dto, Input.SelectedAmenityIds);

                if (Input.Images != null && Input.Images.Any())
                {
                    var uploadPath = Path.Combine(_environment.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadPath);

                    var imageEntities = new List<AccommodationImage>();

                    foreach (var image in Input.Images)
                    {
                        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(image.FileName)}";
                        var filePath = Path.Combine(uploadPath, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                        }

                        imageEntities.Add(new AccommodationImage
                        {
                            AccommodationId = Input.AccommodationId,
                            ImageUrl = $"/uploads/{fileName}",
                            UploadedAt = DateTime.UtcNow
                        });
                    }

                    await _imageService.AddImagesAsync(imageEntities);
                }

                _logger.LogInformation("Accommodation ID {Id} updated successfully", Input.AccommodationId);
                Message = "Listing updated successfully!";
                return RedirectToPage("/dashboard/landlord/mylistings");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating accommodation ID {Id}", Input.AccommodationId);
                ModelState.AddModelError("", "An error occurred while updating the listing.");
                await LoadFormOptionsAsync();
                return Page();
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            var accommodationId = Input.AccommodationId;

            if (accommodationId <= 0)
            {
                _logger.LogWarning("Invalid accommodation ID received for deletion: {Id}", accommodationId);
                return NotFound();
            }

            try
            {
                _logger.LogInformation("Attempting to delete accommodation ID {Id}", accommodationId);
                await _accommodationService.DeleteAsync(accommodationId);
                TempData["SuccessMessage"] = "Listing deleted successfully.";
                _logger.LogInformation("Accommodation ID {Id} deleted successfully", accommodationId);
                return RedirectToPage("/dashboard/landlord/mylistings");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting accommodation ID {Id}", accommodationId);
                TempData["ErrorMessage"] = "An error occurred while deleting the listing.";
                return RedirectToPage("/dashboard/landlord/mylistings");
            }
        }

        private async Task LoadFormOptionsAsync()
        {
            Input.AccommodationTypes = await _typeService.GetAllAsync();
            Input.Universities = await _universityService.GetAllAsync();
            Input.Amenities = await _amenityService.GetAllAsync();
        }
    }
}
