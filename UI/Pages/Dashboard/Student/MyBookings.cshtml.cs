using BLL.DTOs.Accommodation;
using BLL.Interfaces;
using BLL.Services;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace UI.Pages.Dashboard.Student
{
    [Authorize(Roles = "Student")]
    public class MyBookingsModel : PageModel
    {
        private readonly IAccommodationService _accommodationService;
        private readonly IBookingService _bookingService;

        public MyBookingsModel(
            IAccommodationService accommodationService,
            IBookingService bookingService)
        {
            _accommodationService = accommodationService;
            _bookingService = bookingService;
        }


        public List<AccommodationBookingDto> Accommodations { get; set; } = new();

        public async Task OnGetAsync()
        {
            var studentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Accommodations = (await _accommodationService.GetBookingsByStudentUserIdAsync(studentUserId)).ToList();
        }


        public async Task<IActionResult> OnPostAcceptAsync(int id)
        {
            await _bookingService.UpdateStatusAsync(id, "Accepted");
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectAsync(int id)
        {
            await _bookingService.UpdateStatusAsync(id, "Rejected");
            return RedirectToPage();
        }

    }
}
