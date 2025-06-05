using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UI.Pages.Dashboard
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (User.IsInRole("Landlord"))
                return RedirectToPage("/Dashboard/Landlord/Dashboard");

            if (User.IsInRole("Student"))
                return RedirectToPage("/Dashboard/Student/Dashboard");
            if (User.IsInRole("Management"))
                return RedirectToPage("/Dashboard/Management");

            return RedirectToPage("/AccessDenied");
        }
    }
}
