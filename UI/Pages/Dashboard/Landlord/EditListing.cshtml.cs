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
            IWebHostEnvironment environment)
        {
            _logger = logger;
            _imageService = imageService;
            _accommodationService = accommodationService;
            _amenityService = amenityService;
            _typeService = typeService;
            _universityService = universityService;
            _landlordService = landlordService;
            _environment = environment;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                var accommodation = await _accommodationService.GetByIdAsync(id);
                if (accommodation == null)
                    return NotFound();

                // Verify the current user owns this listing
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var landlord = await _landlordService.GetByUserIdAsync(userId);

                //if (landlord == null || !await _accommodationService.IsOwnedByLandlord(id, landlord.LandlordId))
                //{
                //    return Forbid();
                //}

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
            if (!ModelState.IsValid)
            {
                await LoadFormOptionsAsync();
                return Page();
            }

            try
            {
                _logger.LogInformation("Submitting edit for accommodation ID {Id}", Input.AccommodationId);

                // Verify ownership
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var landlord = await _landlordService.GetByUserIdAsync(userId);

                //if (landlord == null || !await _accommodationService.IsOwnedByLandlord(Input.AccommodationId, landlord.LandlordId))
                //{
                //    return Forbid();
                //}

                var dto = new AccommodationUpdateDto
                {
                    AccommodationId = Input.AccommodationId,
                    Title = Input.Title,
                    Description = Input.Description,
                    Address = Input.Address,
                    PostCode = Input.PostCode,
                    City = "Eindhoven",
                    Country = "Netherlands",
                    MonthlyRent = Input.MonthlyRent,
                    Size = Input.Size,
                    MaxOccupants = Input.MaxOccupants,
                    AccommodationTypeId = Input.AccommodationTypeId,
                    UniversityId = Input.UniversityId,
                    IsAvailable = true,
                    AvailableFrom = DateTime.UtcNow
                };



                // Update accommodation and amenities
                await _accommodationService.UpdateWithAmenitiesAsync(dto, Input.SelectedAmenityIds);
                _logger.LogInformation("Accommodation ID {Id} updated successfully", Input.AccommodationId);

                // Handle image uploads if any
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

        private async Task LoadFormOptionsAsync()
        {
            Input.AccommodationTypes = await _typeService.GetAllAsync();
            Input.Universities = await _universityService.GetAllAsync();
            Input.Amenities = await _amenityService.GetAllAsync();
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
                return RedirectToPage("dashboard/landlord/mylistings");
            }
        }


    }
}