using BLL.DTOs.Accommodation;
using BLL.DTOs.Shared;
using BLL.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using UI.ViewModels;

namespace UI.Pages.Dashboard.Landlord
{
    [Authorize(Roles = "Landlord")]
    public class CreateListingModel : PageModel
    {
        private readonly IAccommodationService _accommodationService;
        private readonly IAmenityService _amenityService;
        private readonly IAccommodationTypeService _typeService;
        private readonly IUniversityService _universityService;
        private readonly ILandlordService _landlordService;
        private readonly IAccommodationImageService _imageService; // ✅ NEW
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<CreateListingModel> _logger;

        [BindProperty]
        public AccommodationViewModel Input { get; set; } = new();

        public CreateListingModel(
            IAccommodationService accommodationService,
            IAmenityService amenityService,
            IAccommodationTypeService typeService,
            IUniversityService universityService,
            ILandlordService landlordService,
            IAccommodationImageService imageService,
            IWebHostEnvironment environment,
            ILogger<CreateListingModel> logger)
        {
            _accommodationService = accommodationService;
            _amenityService = amenityService;
            _typeService = typeService;
            _universityService = universityService;
            _landlordService = landlordService;
            _imageService = imageService; 
            _environment = environment;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            _logger.LogDebug("Landlord accessed Create Listing page.");
            await LoadFormOptionsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid listing data submitted: {@Input}", Input);
                await LoadFormOptionsAsync();
                return Page();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var landlord = await _landlordService.GetByUserIdAsync(userId);
            if (landlord == null)
            {
                _logger.LogWarning("Landlord not found for user ID: {UserId}", userId);
                return Forbid();
            }

            var dto = new AccommodationCreateDto
            {
                Title = Input.Title,
                Description = Input.Description,
                Address = Input.Address,
                PostCode = Input.PostCode,
                City = "Eindhoven",
                Country = "Netherlands",
                MonthlyRent = Input.MonthlyRent,
                Size = Input.Size,
                MaxOccupants = Input.MaxOccupants,
                LandlordId = landlord.LandlordId,
                AccommodationTypeId = Input.AccommodationTypeId,
                UniversityId = Input.UniversityId,
                AmenityIds = Input.SelectedAmenityIds,
                Images = Input.Images
            };

            try
            {
                _logger.LogInformation("Creating accommodation listing for landlord ID: {LandlordId}", landlord.LandlordId);
                var accId = await _accommodationService.CreateAsync(dto, Input.SelectedAmenityIds);

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
                            AccommodationId = accId,
                            ImageUrl = $"/uploads/{fileName}",
                            UploadedAt = DateTime.UtcNow
                        });
                    }

                    
                    await _imageService.AddImagesAsync(imageEntities);
                    _logger.LogInformation("Uploaded {Count} image(s) for accommodation ID: {AccommodationId}", imageEntities.Count, accId);
                }

                _logger.LogInformation("Accommodation listing created successfully. ID: {AccommodationId}", accId);
                return RedirectToPage("/Dashboard/Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating accommodation listing for landlord ID: {LandlordId}", landlord.LandlordId);
                ModelState.AddModelError(string.Empty, "An error occurred while creating the listing.");
                await LoadFormOptionsAsync();
                return Page();
            }
        }

        private async Task LoadFormOptionsAsync()
        {
            _logger.LogDebug("Loading form dropdown options for Create Listing page.");
            Input.AccommodationTypes = await _typeService.GetAllAsync();
            Input.Universities = await _universityService.GetAllAsync();
            Input.Amenities = await _amenityService.GetAllAsync();
        }
    }
}
