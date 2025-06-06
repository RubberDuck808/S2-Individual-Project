using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UI.Pages.Listings
{
    [Authorize(Roles = "Landlord")]
    public class EditListingModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
