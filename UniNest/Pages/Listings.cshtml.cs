using Microsoft.AspNetCore.Mvc.RazorPages;
using UniNest.BLL.DTOs.Accommodation;
using UniNest.BLL.Interfaces;

namespace UniNest.Pages
{
    public class ListingsModel : PageModel
    {
        private readonly IAccommodationService _accommodationService;

        public ListingsModel(IAccommodationService accommodationService)
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
