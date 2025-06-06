using BLL.DTOs.Accommodation;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace UI.Pages
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
            Accommodations = (await _accommodationService.GetIndexAsync()).ToList();
        }
    }
}
