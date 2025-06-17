using BLL.DTOs.Application;
using BLL.Interfaces;
using BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;



namespace UI.Pages.Dashboard.Student
{
    [Authorize(Roles = "Student")]
    public class MyApplicationsModel : PageModel
    {
        private readonly IApplicationService _applicationService;
        private readonly IAccommodationService _accommodationService;
        private readonly IStudentService _studentService;
        private readonly ILogger<MyApplicationsModel> _logger;

        public MyApplicationsModel(IApplicationService applicationService, IStudentService studentService, ILogger<MyApplicationsModel> logger, IAccommodationService accommodationService)
        {
            _applicationService = applicationService;
            _studentService = studentService;
            _accommodationService = accommodationService;
            _logger = logger;
        }

        public List<AppliedAccommodationDto> Accommodations { get; set; } = new();

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

                _logger.LogInformation("Fetching applications for student ID: {StudentId}", student.StudentId);

               
                Accommodations = (await _accommodationService.GetApplicationByStudentUserIdAsync(userId)).ToList();

                _logger.LogInformation("{Count} applications found for student ID: {StudentId}", Accommodations.Count, student.StudentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while loading applications for user ID: {UserId}", userId);
                ModelState.AddModelError(string.Empty, "Failed to load your applications.");
            }
        }

    }
}
