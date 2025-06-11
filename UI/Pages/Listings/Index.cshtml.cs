using Microsoft.AspNetCore.Mvc.RazorPages;
using BLL.Interfaces;
using BLL.DTOs.Accommodation;
using BLL.DTOs.Shared;
using Microsoft.AspNetCore.Mvc;

namespace UI.Pages.Listings
{
    public class IndexModel : PageModel
    {
        private readonly IAccommodationService _accommodationService;
        private readonly IUniversityService _universityService;
        private readonly IAccommodationTypeService _typeService;

        public IndexModel(
            IAccommodationService accommodationService,
            IUniversityService universityService,
            IAccommodationTypeService typeService)
        {
            _accommodationService = accommodationService;
            _universityService = universityService;
            _typeService = typeService;
        }

        public List<AccommodationDto> Accommodations { get; set; } = new();
        public List<UniversityDto> Universities { get; set; } = new();
        public List<AccommodationTypeDto> AccommodationTypes { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? UniversityId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? TypeId { get; set; }

        public async Task OnGetAsync()
        {
            Accommodations = (await _accommodationService.GetAllAsync()).ToList();
            Universities = (await _universityService.GetAllAsync()).ToList();
            AccommodationTypes = (await _typeService.GetAllAsync()).ToList();

            if (!string.IsNullOrWhiteSpace(Search))
            {
                Accommodations = Accommodations.Where(a =>
                    a.Title.Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                    a.City.Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                    a.Address.Contains(Search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (UniversityId.HasValue)
            {
                Accommodations = Accommodations.Where(a => a.UniversityId == UniversityId.Value).ToList();
            }

            if (TypeId.HasValue)
            {
                Accommodations = Accommodations.Where(a => a.AccommodationTypeId == TypeId.Value).ToList();
            }
        }
    }
}
