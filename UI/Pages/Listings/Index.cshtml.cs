using Microsoft.AspNetCore.Mvc.RazorPages;
using BLL.Interfaces;
using BLL.DTOs.Accommodation;
using BLL.DTOs.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace UI.Pages.Listings
{
    public class IndexModel : PageModel
    {
        private readonly IAccommodationService _accommodationService;
        private readonly IUniversityService _universityService;
        private readonly IAccommodationTypeService _typeService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(
            IAccommodationService accommodationService,
            IUniversityService universityService,
            IAccommodationTypeService typeService,
            ILogger<IndexModel> logger)
        {
            _accommodationService = accommodationService;
            _universityService = universityService;
            _typeService = typeService;
            _logger = logger;
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
            _logger.LogInformation("Listing page accessed. Search: {Search}, UniversityId: {UniversityId}, TypeId: {TypeId}",
                Search, UniversityId, TypeId);

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

            _logger.LogInformation("Filtered results: {Count} accommodations found.", Accommodations.Count);
        }
    }
}
