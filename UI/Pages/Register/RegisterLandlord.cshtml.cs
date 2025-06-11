using BLL.DTOs.Landlord;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging; // Required for ILogger
using System.ComponentModel.DataAnnotations;

namespace UI.Pages.Register
{
    public class RegisterLandlordModel : PageModel
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<RegisterLandlordModel> _logger;

        public RegisterLandlordModel(IAccountService accountService, ILogger<RegisterLandlordModel> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        [BindProperty]
        public LandlordInputModel LandlordInput { get; set; } = new();

        public class LandlordInputModel
        {
            [Required]
            public string FirstName { get; set; }

            public string? MiddleName { get; set; }

            [Required]
            public string LastName { get; set; }

            [Required, EmailAddress]
            public string Email { get; set; }

            [Required]
            public string PhoneNumber { get; set; }

            public string? CompanyName { get; set; }

            public string? TaxIdentificationNumber { get; set; }

            [Required]
            public string Password { get; set; }

            [Required, Compare("Password")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid registration attempt for landlord: {@Input}", LandlordInput);
                return Page();
            }

            var dto = new LandlordRegistrationDto
            {
                FirstName = LandlordInput.FirstName,
                MiddleName = LandlordInput.MiddleName,
                LastName = LandlordInput.LastName,
                Email = LandlordInput.Email,
                PhoneNumber = LandlordInput.PhoneNumber,
                Password = LandlordInput.Password,
                CompanyName = LandlordInput.CompanyName,
                TaxIdentificationNumber = LandlordInput.TaxIdentificationNumber
            };

            try
            {
                _logger.LogInformation("Attempting to register landlord with email: {Email}", dto.Email);
                await _accountService.RegisterLandlordAsync(dto);
                _logger.LogInformation("Landlord registration successful for email: {Email}", dto.Email);
                return RedirectToPage("/Dashboard/Landlord/Account");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering landlord with email: {Email}", dto.Email);
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
    }
}
