using BLL.DTOs.Accommodation;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace UI.Pages.Dashboard.Student
{
    [Authorize(Roles = "Student")]
    public class MyBookingsModel : PageModel
    {
        private readonly IAccommodationService _accommodationService;
        private readonly IBookingService _bookingService;
        private readonly ILogger<MyBookingsModel> _logger;

        public MyBookingsModel(
            IAccommodationService accommodationService,
            IBookingService bookingService,
            ILogger<MyBookingsModel> logger)
        {
            _accommodationService = accommodationService;
            _bookingService = bookingService;
            _logger = logger;
        }

        public List<AccommodationBookingDto> Accommodations { get; set; } = new();

        public async Task OnGetAsync()
        {
            var studentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(studentUserId))
            {
                _logger.LogWarning("Student user ID claim missing on booking page access.");
                return;
            }

            try
            {
                _logger.LogInformation("Fetching bookings for student user ID: {UserId}", studentUserId);
                Accommodations = (await _accommodationService.GetBookingsByStudentUserIdAsync(studentUserId)).ToList();
                _logger.LogInformation("{Count} bookings retrieved for user ID: {UserId}", Accommodations.Count, studentUserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load bookings for user ID: {UserId}", studentUserId);
                ModelState.AddModelError(string.Empty, "Unable to load your bookings at the moment.");
            }
        }

        public async Task<IActionResult> OnPostAcceptAsync(int id)
        {
            try
            {
                _logger.LogInformation("Student accepted booking ID: {BookingId}", id);
                await _bookingService.UpdateStatusAsync(id, "Accepted");
                TempData["SuccessMessage"] = "Booking accepted.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accepting booking ID: {BookingId}", id);
                TempData["ErrorMessage"] = "Could not accept the booking.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectAsync(int id)
        {
            try
            {
                _logger.LogInformation("Student rejected booking ID: {BookingId}", id);
                await _bookingService.UpdateStatusAsync(id, "Rejected");
                TempData["SuccessMessage"] = "Booking rejected.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting booking ID: {BookingId}", id);
                TempData["ErrorMessage"] = "Could not reject the booking.";
            }

            return RedirectToPage();
        }
    }
}
