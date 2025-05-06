using Microsoft.AspNetCore.Mvc.RazorPages;
using UniNest.BLL.DTOs.Landlord;
using UniNest.BLL.Interfaces;

namespace UniNest.Pages
{
    public class LandlordModel : PageModel
    {
        private readonly ILandlordService _landlordService;

        public LandlordModel(ILandlordService landlordService)
        {
            _landlordService = landlordService;
        }

        public List<LandlordDto> VerifiedLandlords { get; set; } = new();
        public List<LandlordDto> UnverifiedLandlords { get; set; } = new();

        public async Task OnGetAsync()
        {
            var all = (await _landlordService.GetAllAsync()).ToList();
            VerifiedLandlords = all.Where(l => l.IsVerified).ToList();
            UnverifiedLandlords = all.Where(l => !l.IsVerified).ToList();
        }
    }
}
