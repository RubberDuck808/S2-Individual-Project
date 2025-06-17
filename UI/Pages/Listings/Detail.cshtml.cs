using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BLL.Interfaces;
using BLL.DTOs.Accommodation;
using BLL.DTOs.Application;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace UI.Pages.Listings
{
    public class DetailModel : PageModel
    {
        private readonly IAccommodationService _accommodationService;
        private readonly IApplicationService _applicationService;
        private readonly IStudentService _studentService;
        private readonly ILogger<DetailModel> _logger;

        public DetailModel(
            IAccommodationService accommodationService,
            IApplicationService applicationService,
            IStudentService studentService,
            ILogger<DetailModel> logger)
        {
            _accommodationService = accommodationService;
            _applicationService = applicationService;
            _studentService = studentService;
            _logger = logger;
        }

        public AccommodationDto? Accommodation { get; set; }

        public string? Message => TempData["Message"]?.ToString();

        public bool AlreadyApplied { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                _logger.LogInformation("Fetching accommodation details for ID: {Id}", id);
                Accommodation = await _accommodationService.GetByIdAsync(id);

                if (Accommodation == null)
                {
                    _logger.LogWarning("Accommodation not found for ID: {Id}", id);
                    return RedirectToPage("/NotFound");
                }

                if (User.Identity.IsAuthenticated && User.IsInRole("Student"))
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var student = await _studentService.GetByUserIdAsync(userId);
                    if (student != null)
                    {
                        var apps = await _applicationService.GetByStudentAsync(student.StudentId);
                        AlreadyApplied = apps.Any(a => a.AccommodationId == id);
                    }
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading accommodation details for ID: {Id}", id);
                return RedirectToPage("/NotFound");
            }
        }

        public async Task<IActionResult> OnPostApplyAsync(int accommodationId)
        {
            if (!User.Identity.IsAuthenticated || !User.IsInRole("Student"))
            {
                _logger.LogWarning("Unauthorized access attempt to apply for accommodation ID: {Id}", accommodationId);
                return Forbid();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("No user ID found in claims for apply attempt to accommodation ID: {Id}", accommodationId);
                return Unauthorized();
            }

            var student = await _studentService.GetByUserIdAsync(userId);
            if (student == null)
            {
                _logger.LogWarning("Student profile not found for user ID: {UserId}", userId);
                return NotFound("Student profile not found.");
            }

            var existingApplications = await _applicationService.GetByStudentAsync(student.StudentId);
            if (existingApplications.Any(a => a.AccommodationId == accommodationId))
            {
                _logger.LogInformation("Duplicate application attempt by student ID: {StudentId} for accommodation ID: {AccommodationId}", student.StudentId, accommodationId);
                TempData["Message"] = "You already applied for this listing.";
                return RedirectToPage(new { id = accommodationId });
            }

            var dto = new ApplicationCreateDto
            {
                StudentId = student.StudentId,
                AccommodationId = accommodationId,
                StatusId = 1,
                ApplicationDate = DateTime.UtcNow
            };

            try
            {
                await _applicationService.CreateAsync(dto);
                _logger.LogInformation("Student ID: {StudentId} successfully applied to accommodation ID: {AccommodationId}", student.StudentId, accommodationId);
                TempData["Message"] = "Application submitted successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying for accommodation ID: {AccommodationId} by student ID: {StudentId}", accommodationId, student.StudentId);
                TempData["Message"] = "Something went wrong while submitting your application.";
            }

            return RedirectToPage(new { id = accommodationId });
        }
    }
}
