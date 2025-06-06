using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BLL.Interfaces;
using BLL.DTOs.Accommodation;

namespace UI.Pages.Listings
{
    public class DetailModel : PageModel
    {
        private readonly IAccommodationService _accommodationService;

        public DetailModel(IAccommodationService accommodationService)
        {
            _accommodationService = accommodationService;
        }

        public AccommodationDto? Accommodation { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                Accommodation = await _accommodationService.GetByIdAsync(id);
                return Page();
            }
            catch
            {
                
                return RedirectToPage("/NotFound");
            }
        }
    }
}
