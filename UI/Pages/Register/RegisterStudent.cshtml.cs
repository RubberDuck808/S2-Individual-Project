using BLL.DTOs.Student;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace UI.Pages.Register
{
    public class RegisterStudentModel : PageModel
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<RegisterStudentModel> _logger;

        public RegisterStudentModel(IAccountService accountService, ILogger<RegisterStudentModel> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        [BindProperty]
        public StudentInputModel StudentInput { get; set; } = new();

        public class StudentInputModel
        {
            [Required] public string FirstName { get; set; }
            public string? MiddleName { get; set; }
            [Required] public string LastName { get; set; }

            [Required, EmailAddress] public string Email { get; set; }
            [Required] public string PhoneNumber { get; set; }
            [Required] public DateTime DateOfBirth { get; set; }

            public string? EmergencyContact { get; set; }
            public string? EmergencyPhone { get; set; }

            [Required] public string Password { get; set; }
            [Required, Compare("Password")] public string ConfirmPassword { get; set; }
        }


        public void OnGet()
        {
            // Only set if empty (to avoid overwriting user input after validation errors)
            if (StudentInput.DateOfBirth == default)
            {
                StudentInput.DateOfBirth = new DateTime(2000, 1, 1);
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation("RegisterStudent POST triggered");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state invalid for student registration");
                return Page();
            }

            var dto = new StudentRegistrationDto
            {
                FirstName = StudentInput.FirstName,
                MiddleName = StudentInput.MiddleName,
                LastName = StudentInput.LastName,
                Email = StudentInput.Email,
                PhoneNumber = StudentInput.PhoneNumber,
                DateOfBirth = StudentInput.DateOfBirth,
                Password = StudentInput.Password,
                EmergencyContact = StudentInput.EmergencyContact,
                EmergencyPhone = StudentInput.EmergencyPhone,
                ProfileImageUrl = "https://as2.ftcdn.net/jpg/00/64/67/27/1000_F_64672736_U5kpdGs9keUll8CRQ3p3YaEv2M6qkVY5.jpg"
            };

            try
            {
                _logger.LogInformation("Attempting to register student: {Email}", dto.Email);
                await _accountService.RegisterStudentAsync(dto);
                _logger.LogInformation("Student registration successful: {Email}", dto.Email);
                return RedirectToPage("/");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Student registration failed for {Email}", dto.Email);
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
    }
}
