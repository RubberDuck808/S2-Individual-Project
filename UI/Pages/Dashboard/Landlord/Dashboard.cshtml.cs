using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace UI.Pages.Dashboard.Landlord
{
    [Authorize(Roles = "Landlord")]
    public class IndexModel : PageModel
    {
        public void OnGet() { }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            await HttpContext.SignOutAsync("UniNestAuth");
            return RedirectToPage("/Index");
        }
    }
}
