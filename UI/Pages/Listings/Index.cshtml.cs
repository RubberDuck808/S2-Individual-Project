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
        private readonly IGeoLocationService _geoLocationService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(
            IAccommodationService accommodationService,
            IUniversityService universityService,
            IAccommodationTypeService typeService,
            IGeoLocationService geoLocationService,
            ILogger<IndexModel> logger)
        {
            _accommodationService = accommodationService;
            _universityService = universityService;
            _typeService = typeService;
            _geoLocationService = geoLocationService;
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

        [BindProperty(SupportsGet = true)]
        public int? Radius { get; set; } // in km

        public async Task OnGetAsync()
        {
            _logger.LogInformation("Listing page accessed. Search: {Search}, UniversityId: {UniversityId}, TypeId: {TypeId}, Radius: {Radius}",
                Search, UniversityId, TypeId, Radius);

            Accommodations = (await _accommodationService.GetAllAsync()).ToList();
            _logger.LogInformation("Initial accommodations count: {Count}", Accommodations.Count);

            Universities = (await _universityService.GetAllAsync()).ToList();
            AccommodationTypes = (await _typeService.GetAllAsync()).ToList();

            if (UniversityId.HasValue)
            {
                var before = Accommodations.Count;
                Accommodations = Accommodations.Where(a => a.UniversityId == UniversityId.Value).ToList();
                var after = Accommodations.Count;

                _logger.LogInformation("University filter applied: {Before} → {After}", before, after);

                if (after == 0)
                {
                    var uniName = Universities.FirstOrDefault(u => u.UniversityId == UniversityId.Value)?.Name ?? "Unknown";
                    _logger.LogWarning("No accommodations found for UniversityId {Id} ({Name})", UniversityId, uniName);
                }
            }

            if (TypeId.HasValue)
            {
                var before = Accommodations.Count;
                Accommodations = Accommodations.Where(a => a.AccommodationTypeId == TypeId.Value).ToList();
                var after = Accommodations.Count;

                _logger.LogInformation("Type filter applied: {Before} → {After}", before, after);

                if (after == 0)
                {
                    var typeName = AccommodationTypes.FirstOrDefault(t => t.AccommodationTypeId == TypeId.Value)?.Name ?? "Unknown";
                    _logger.LogWarning("No accommodations found for TypeId {Id} ({Name})", TypeId, typeName);
                }
            }

            bool locationFilterApplied = false;

            if (!string.IsNullOrWhiteSpace(Search))
            {
                var coords = await _geoLocationService.GetCoordinatesFromAddressAsync(Search);
                if (coords != null)
                {
                    var lat = coords.Value.lat;
                    var lng = coords.Value.lng;

                    var nearby = Accommodations
                        .Where(a => a.Latitude.HasValue && a.Longitude.HasValue)
                        .Select(a => new
                        {
                            Accommodation = a,
                            Distance = _geoLocationService.CalculateDistanceKm(lat, lng, a.Latitude.Value, a.Longitude.Value)
                        })
                        .OrderBy(x => x.Distance);

                    if (Radius.HasValue)
                    {
                        var before = Accommodations.Count;
                        Accommodations = nearby
                            .Where(x => x.Distance <= Radius.Value)
                            .Select(x => x.Accommodation)
                            .ToList();
                        _logger.LogInformation("Radius filter applied: {Before} → {After}", before, Accommodations.Count);
                    }
                    else
                    {
                        Accommodations = nearby
                            .Take(10)
                            .Select(x => x.Accommodation)
                            .ToList();
                        _logger.LogInformation("Top 10 closest accommodations selected");
                    }

                    locationFilterApplied = true;
                }

                if (!locationFilterApplied)
                {
                    _logger.LogWarning("GeoLocation lookup failed for search: {Search}", Search);

                    Accommodations = Accommodations.Where(a =>
                        a.Title.Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                        a.City.Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                        a.Address.Contains(Search, StringComparison.OrdinalIgnoreCase)).ToList();

                    if (Accommodations.Count < 3)
                    {
                        var (city, postcode) = await _geoLocationService.GetCityAndPostalCodeFromAddressAsync(Search);
                        if (!string.IsNullOrWhiteSpace(city) || !string.IsNullOrWhiteSpace(postcode))
                        {
                            var normalizedPostcode = postcode?.Replace(" ", "").ToLower();

                            Accommodations = Accommodations.Union(
                                Accommodations.Where(a =>
                                    (!string.IsNullOrWhiteSpace(city) && a.City.Contains(city, StringComparison.OrdinalIgnoreCase)) ||
                                    (!string.IsNullOrWhiteSpace(normalizedPostcode) &&
                                        a.PostCode.Replace(" ", "").ToLower().Contains(normalizedPostcode)))
                            ).Distinct().ToList();

                            _logger.LogInformation("Fallback search used city/postcode: {City}, {PostCode}", city, postcode);
                        }
                    }
                }
            }

            _logger.LogInformation("Filtered results: {Count} accommodations found.", Accommodations.Count);
        }


    }
}
