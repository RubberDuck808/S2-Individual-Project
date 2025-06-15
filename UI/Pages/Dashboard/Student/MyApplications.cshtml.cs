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
    public class MyApplicationsModel : PageModel
    {
        private readonly IAccommodationService _accommodationService;
        private readonly ILogger<MyApplicationsModel> _logger;

        public MyApplicationsModel(IAccommodationService accommodationService, ILogger<MyApplicationsModel> logger)
        {
            _accommodationService = accommodationService;
            _logger = logger;
        }

        public List<AppliedAccommodationDto> Accommodations { get; set; } = new();

        public async Task OnGetAsync()
        {
            var studentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(studentUserId))
            {
                _logger.LogWarning("No student user ID found in claims.");
                return;
            }

            try
            {
                _logger.LogInformation("Fetching applications for student user ID: {UserId}", studentUserId);
                Accommodations = (await _accommodationService.GetByStudentUserIdAsync(studentUserId)).ToList();
                _logger.LogInformation("{Count} applications found for user ID: {UserId}", Accommodations.Count, studentUserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while loading applications for user ID: {UserId}", studentUserId);
                ModelState.AddModelError(string.Empty, "Failed to load your applications.");
            }
        }
    }
}
