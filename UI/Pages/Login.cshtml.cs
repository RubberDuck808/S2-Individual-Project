using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace UI.Pages
{
    public class LoginModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        [BindProperty]
        public string Email { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string RegisterEmail { get; set; }

        [BindProperty]
        public string RegisterPassword { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        public string ErrorMessage { get; set; }

        public LoginModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(Email, Password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToPage("./Index"); 
                }
                else
                {
                    ErrorMessage = "Invalid login attempt.";
                    return Page();
                }
            }

            ErrorMessage = "Invalid input. Please correct the errors.";
            return Page();
        }

        public async Task<IActionResult> OnPostRegisterAsync()
        {
            if (ModelState.IsValid)
            {
                if (RegisterPassword != ConfirmPassword)
                {
                    ErrorMessage = "Password and confirm password do not match.";
                    return Page();
                }

                var user = new IdentityUser { UserName = RegisterEmail, Email = RegisterEmail };
                var result = await _userManager.CreateAsync(user, RegisterPassword);

                if (result.Succeeded)
                {
                    
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToPage("./Index"); 
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    ErrorMessage = "Registration failed. Please check the errors.";
                    return Page();
                }
            }

            ErrorMessage = "Invalid input. Please correct the errors.";
            return Page();
        }
    }
}