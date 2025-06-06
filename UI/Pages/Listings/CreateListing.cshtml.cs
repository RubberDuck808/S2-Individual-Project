using BLL.DTOs.Accommodation;
using BLL.Interfaces;
using BLL.Services;
using DAL.Interfaces;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;
using UI.ViewModels;

namespace UI.Pages.Listings
{
    [Authorize(Roles = "Landlord")]
    public class CreateListingModel : PageModel
    {
        private readonly IAccommodationService _accommodationService;
        private readonly IAmenityRepository _amenityRepository;
        private readonly IAccommodationTypeRepository _typeRepository;
        private readonly IUniversityRepository _universityRepository;
        private readonly ILandlordRepository _landlordRepository;
        private readonly IWebHostEnvironment _environment;

        [BindProperty]
        public CreateAccommodationViewModel Input { get; set; } = new();

        public CreateListingModel(
            IAccommodationService accommodationService,
            IAmenityRepository amenityRepository,
            IAccommodationTypeRepository typeRepository,
            IUniversityRepository universityRepository,
            ILandlordRepository landlordRepository,
            IWebHostEnvironment environment)
        {
            _accommodationService = accommodationService;
            _amenityRepository = amenityRepository;
            _typeRepository = typeRepository;
            _universityRepository = universityRepository;
            _landlordRepository = landlordRepository;
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
            var landlord = await _landlordRepository.GetByUserIdAsync(userId);
            if (landlord == null)
            {
                return Forbid();
            }

            var dto = new AccommodationCreateDto
            {
                Title = Input.Title,
                Description = Input.Description,
                Address = Input.Address,
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
            Input.AccommodationTypes = await _typeRepository.GetAllAsync();
            Input.Universities = await _universityRepository.GetAllAsync();
            Input.Amenities = await _amenityRepository.GetAllAsync();
        }
    }
}
