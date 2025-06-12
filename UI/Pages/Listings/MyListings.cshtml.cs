using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BLL.Interfaces;
using BLL.DTOs.Accommodation;
using System.Security.Claims;

namespace UI.Pages.Landlord
{
    [Authorize(Roles = "Landlord")]
    public class MyListingsModel : PageModel
    {
        private readonly IAccommodationService _accommodationService;
        private readonly IApplicationService _applicationService;
        private readonly IBookingService _bookingService;

        public MyListingsModel(
            IAccommodationService accommodationService,
            IApplicationService applicationService,
            IBookingService bookingService)
        {
            _accommodationService = accommodationService;
            _applicationService = applicationService;
            _bookingService = bookingService;
        }

        public List<LandlordAccommodationDto> Accommodations { get; set; } = new();

        public async Task OnGetAsync()
        {
            var landlordUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Accommodations = (await _accommodationService.GetByLandlordUserIdAsync(landlordUserId)).ToList();
        }

        public async Task<IActionResult> OnPostChooseAsync(int id)
        {
            var applications = await _applicationService.GetByAccommodationIdAsync(id);
            if (applications == null || !applications.Any())
            {
                TempData["Error"] = "No applicants found for this listing.";
                return RedirectToPage();
            }

            var random = new Random();
            var selectedApp = applications[random.Next(applications.Count)];

            await _applicationService.SelectApplicantAsync(selectedApp.ApplicationId, id);
            await _bookingService.CreateAsync(selectedApp.StudentId, id, selectedApp.ApplicationId);

            TempData["Success"] = "An applicant was selected and a booking was created.";
            return RedirectToPage();
        }
    }
}
