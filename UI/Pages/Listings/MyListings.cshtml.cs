using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using BLL.Interfaces;
using BLL.DTOs.Accommodation;
using System.Security.Claims;

namespace UI.Pages.Landlord
{
    [Authorize(Roles = "Landlord")]
    public class MyListingsModel : PageModel
    {
        private readonly IAccommodationService _accommodationService;

        public MyListingsModel(IAccommodationService accommodationService)
        {
            _accommodationService = accommodationService;
        }

        public List<LandlordAccommodationDto> Accommodations { get; set; } = new();

        public async Task OnGetAsync()
        {
            var landlordUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Accommodations = (await _accommodationService.GetByLandlordUserIdAsync(landlordUserId)).ToList();
        }
    }
}
