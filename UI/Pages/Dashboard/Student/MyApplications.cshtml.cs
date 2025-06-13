using BLL.DTOs.Accommodation;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace UI.Pages.Dashboard.Student
{
    [Authorize(Roles = "Student")]
    public class MyApplicationsModel : PageModel
    {
        private readonly IAccommodationService _accommodationService;

        public MyApplicationsModel(IAccommodationService accommodationService)
        {
            _accommodationService = accommodationService;
        }

        public List<AppliedAccommodationDto> Accommodations { get; set; } = new();

        public async Task OnGetAsync()
        {
            var studentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Accommodations = (await _accommodationService.GetByStudentUserIdAsync(studentUserId)).ToList();
        }
    }
}
