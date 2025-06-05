using Microsoft.AspNetCore.Mvc.RazorPages;
using BLL.Interfaces;
using BLL.DTOs.Accommodation;
using Microsoft.AspNetCore.Authorization;

namespace UI.Pages.Listings
{
    public class IndexModel : PageModel
    {
        private readonly IAccommodationService _accommodationService;

        public IndexModel(IAccommodationService accommodationService)
        {
            _accommodationService = accommodationService;
        }

        public List<AccommodationDto> Accommodations { get; set; } = new();

        public async Task OnGetAsync()
        {
            Accommodations = (await _accommodationService.GetAllAsync()).ToList();
        }
    }
}
