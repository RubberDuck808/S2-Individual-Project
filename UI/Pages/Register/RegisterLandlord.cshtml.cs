using BLL.DTOs.Landlord;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace UI.Pages.Register
{
    public class RegisterLandlordModel : PageModel
    {
        private readonly IAccountService _accountService;

        public RegisterLandlordModel(IAccountService accountService)
        {
            _accountService = accountService;
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

            [Required]
            public string Password { get; set; }

            [Required, Compare("Password")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var dto = new LandlordRegistrationDto
            {
                FirstName = LandlordInput.FirstName,
                MiddleName = LandlordInput.MiddleName,
                LastName = LandlordInput.LastName,
                Email = LandlordInput.Email,
                PhoneNumber = LandlordInput.PhoneNumber,
                Password = LandlordInput.Password
            };

            try
            {
                await _accountService.RegisterLandlordAsync(dto);
                return RedirectToPage("/Dashboard/Landlord/Account");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
    }
}
