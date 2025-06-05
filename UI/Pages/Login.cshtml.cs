using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BLL.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace UI.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IAccountService _accountService;

        public LoginModel(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [BindProperty, Required, EmailAddress]
        public string Email { get; set; }

        [BindProperty, Required, DataType(DataType.Password)]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Invalid input. Please check your entries.";
                return Page();
            }

            var (success, userId, role, error) = await _accountService.LoginAsync(Email, Password);

            if (!success)
            {
                ErrorMessage = error;
                return Page();
            }


            return role switch
            {
                "Student" => RedirectToPage("/Register/RegisterStudent"),
                "Landlord" => RedirectToPage("/Register/RegisterLandlord"),
                "Admin" => RedirectToPage("/Admin/Panel"),
                _ => RedirectToPage("/Index")
            };
        }
    }
}
