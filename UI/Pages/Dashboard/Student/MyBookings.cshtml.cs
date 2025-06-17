using BLL.DTOs.Accommodation;
using BLL.DTOs.Application;
using BLL.Interfaces;
using BLL.Services;
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
        private readonly IStudentService _studentService;
        private readonly IBookingService _bookingService;
        private readonly ILogger<MyBookingsModel> _logger;

        public MyBookingsModel(
            IAccommodationService accommodationService,
            IBookingService bookingService,
            IStudentService studentService,
            ILogger<MyBookingsModel> logger)
        {
            _accommodationService = accommodationService;
            _bookingService = bookingService;
            _studentService = studentService;
            _logger = logger;
        }

        public List<AccommodationBookingDto> Accommodations { get; set; } = new();

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("No user ID found in claims.");
                return;
            }

            try
            {
                var student = await _studentService.GetByUserIdAsync(userId);
                if (student == null)
                {
                    _logger.LogWarning("No student found for user ID: {UserId}", userId);
                    return;
                }

                _logger.LogInformation("Fetching bookings for student ID: {StudentId}", student.StudentId);


                Accommodations = (await _accommodationService.GetBookingsByStudentUserIdAsync(userId)).ToList();

                _logger.LogInformation("{Count} bookings found for student ID: {StudentId}", Accommodations.Count, student.StudentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while loading bookings for user ID: {UserId}", userId);
                ModelState.AddModelError(string.Empty, "Failed to load your bookings.");
            }
        }


        public async Task<IActionResult> OnPostAcceptAsync(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var student = await _studentService.GetByUserIdAsync(userId);
            if (student == null) return Forbid();

            var booking = await _bookingService.GetByIdAsync(id);
            if (booking == null || booking.StudentId != student.StudentId)
            {
                _logger.LogWarning("Unauthorized accept attempt. Student ID: {StudentId} Booking ID: {BookingId}", student.StudentId, id);
                return Forbid();
            }

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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var student = await _studentService.GetByUserIdAsync(userId);
            if (student == null) return Forbid();

            var booking = await _bookingService.GetByIdAsync(id);
            if (booking == null || booking.StudentId != student.StudentId)
            {
                _logger.LogWarning("Unauthorized reject attempt. Student ID: {StudentId} Booking ID: {BookingId}", student.StudentId, id);
                return Forbid();
            }

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
