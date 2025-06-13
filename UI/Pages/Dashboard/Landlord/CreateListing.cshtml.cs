using BLL.DTOs.Accommodation;
using BLL.DTOs.Shared;
using BLL.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        private readonly IWebHostEnvironment _environment;

        [BindProperty]
        public AccommodationViewModel Input { get; set; } = new();

        public CreateListingModel(
            IAccommodationService accommodationService,
            IAmenityService amenityService,
            IAccommodationTypeService typeService,
            IUniversityService universityService,
            ILandlordService landlordService,
            IWebHostEnvironment environment)
        {
            _accommodationService = accommodationService;
            _amenityService = amenityService;
            _typeService = typeService;
            _universityService = universityService;
            _landlordService = landlordService;
            _environment = environment;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadFormOptionsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadFormOptionsAsync();
                return Page();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var landlord = await _landlordService.GetByUserIdAsync(userId);
            if (landlord == null)
            {
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

            var accId = await _accommodationService.CreateAccommodationWithAmenitiesAsync(dto, Input.SelectedAmenityIds);

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

                await _accommodationService.AddImagesAsync(imageEntities);
            }

            return RedirectToPage("/Dashboard/Index");
        }

        private async Task LoadFormOptionsAsync()
        {
            Input.AccommodationTypes = await _typeService.GetAllAsync();
            Input.Universities = await _universityService.GetAllAsync();
            Input.Amenities = await _amenityService.GetAllAsync();
        }
    }
}
