using BLL.DTOs.Accommodation;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace UI.Pages.Dashboard.Landlord
{
    [Authorize(Roles = "Landlord")]
    public class MyBookingsModel : PageModel
    {
        private readonly IAccommodationService _accommodationService;
        private readonly ILogger<MyBookingsModel> _logger;

        public MyBookingsModel(
            IAccommodationService accommodationService,
            ILogger<MyBookingsModel> logger)
        {
            _accommodationService = accommodationService;
            _logger = logger;
        }

        public List<AccommodationBookingDto> Accommodations { get; set; } = new();

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("No landlord user ID found.");
                return;
            }

            try
            {
                Accommodations = (await _accommodationService.GetAcceptedBookingsByLandlordUserIdAsync(userId)).ToList();
                _logger.LogInformation("Loaded {Count} accepted bookings for landlord.", Accommodations.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load accepted bookings.");
            }
        }
    }
}
