using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BLL.Interfaces;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

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

            string fullName = await _accountService.GetFullNameByUserIdAsync(userId);

            //  Set logged-in session
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, fullName),
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, Email),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("UniNestAuth", principal);

            return role switch
            {
                "Student" => RedirectToPage("/Dashboard/Index"),
                "Landlord" => RedirectToPage("/Dashboard/Index"),
                "Admin" => RedirectToPage("/Dashboard/Index"),
                _ => RedirectToPage("/Index")
            };
        }

    }
}
