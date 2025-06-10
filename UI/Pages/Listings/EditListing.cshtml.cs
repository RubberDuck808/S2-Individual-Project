using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BLL.DTOs.Accommodation;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using UI.ViewModels;

namespace UI.Pages.Listings
{
    public class EditListingModel : PageModel
    {
        private readonly IAccommodationService _accommodationService;
        private readonly IUniversityService _universityService;
        private readonly IAccommodationTypeService _typeService;
        private readonly IAmenityService _amenityService;

        [BindProperty]
        public AccommodationViewModel Input { get; set; } = new();
        public EditListingModel(
            IAccommodationService accommodationService,
            IUniversityService universityService,
            IAccommodationTypeService typeService,
            IAmenityService amenityService)
        {
            _accommodationService = accommodationService;
            _universityService = universityService;
            _typeService = typeService;
            _amenityService = amenityService;
        }

        [BindProperty]
        public AccommodationUpdateDto Form { get; set; } = new();

        [BindProperty]
        public List<int> SelectedAmenityIds { get; set; } = new();

        public List<SelectListItem> Universities { get; set; } = new();
        public List<SelectListItem> AccommodationTypes { get; set; } = new();
        public List<SelectListItem> AllAmenities { get; set; } = new();


        public string? Message { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var accommodation = await _accommodationService.GetByIdAsync(id);
            if (accommodation == null)
                return NotFound();

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

            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return Page();
            }
            Console.WriteLine($"Post ID: {Input.AccommodationId}, Title: {Input.Title}");


            var updateDto = new AccommodationUpdateDto
            {
                AccommodationId = Input.AccommodationId,
                Title = Input.Title,
                Description = Input.Description,
                Address = Input.Address,
                PostCode = Input.PostCode,
                City = Input.City,
                Country = Input.Country,
                MonthlyRent = Input.MonthlyRent,
                MaxOccupants = Input.MaxOccupants,
                Size = Input.Size,
                AccommodationTypeId = Input.AccommodationTypeId,
                UniversityId = Input.UniversityId,
                IsAvailable = true, 
                AvailableFrom = DateTime.UtcNow 
            };

            await _accommodationService.UpdateAsync(updateDto);
            await _accommodationService.AddAmenitiesAsync(Input.AccommodationId, Input.SelectedAmenityIds);

            Message = "Listing updated successfully!";
            await PopulateDropdowns();
            return Page();
        }


        private async Task PopulateDropdowns()
        {


            var universities = await _universityService.GetAllAsync();
            var types = await _typeService.GetAllAsync();
            var amenities = await _amenityService.GetAllAsync();

            Universities = universities.Select(u => new SelectListItem(u.Name, u.UniversityId.ToString())).ToList();
            AccommodationTypes = types.Select(t => new SelectListItem(t.Name, t.AccommodationTypeId.ToString())).ToList();
            AllAmenities = amenities.Select(a => new SelectListItem(a.Name, a.AmenityId.ToString())).ToList();
        }
    }
}
