using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BLL.Interfaces;
using BLL.DTOs.Accommodation;
using BLL.DTOs.Application;
using System.Security.Claims;

namespace UI.Pages.Listings
{
    public class DetailModel : PageModel
    {
        private readonly IAccommodationService _accommodationService;
        private readonly IApplicationService _applicationService;
        private readonly IStudentService _studentService;

        public DetailModel(
            IAccommodationService accommodationService,
            IApplicationService applicationService,
            IStudentService studentService)
        {
            _accommodationService = accommodationService;
            _applicationService = applicationService;
            _studentService = studentService;
        }

        public AccommodationDto? Accommodation { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                Accommodation = await _accommodationService.GetByIdAsync(id);
                return Page();
            }
            catch
            {
                return RedirectToPage("/NotFound");
            }
        }

        public async Task<IActionResult> OnPostApplyAsync(int accommodationId)
        {
            if (!User.Identity.IsAuthenticated || !User.IsInRole("Student"))
                return Forbid();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var student = await _studentService.GetByUserIdAsync(userId);
            if (student == null)
                return NotFound("Student profile not found.");

            var existingApplications = await _applicationService.GetByStudentAsync(student.StudentId);
            if (existingApplications.Any(a => a.AccommodationId == accommodationId))
            {
                TempData["Message"] = "You already applied for this listing.";
                return RedirectToPage();
            }

            var dto = new ApplicationCreateDto
            {
                StudentId = student.StudentId,
                AccommodationId = accommodationId,
                StatusId = 1, 
                ApplicationDate = DateTime.UtcNow
            };

            await _applicationService.CreateAsync(dto);

            TempData["Message"] = "Application submitted successfully!";
            return RedirectToPage();
        }
    }
}
