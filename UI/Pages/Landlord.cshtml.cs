using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BLL.Interfaces;
using BLL.DTOs.Landlord;

namespace UI.Pages
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




        // Basic
        [BindProperty] public int BasicEditLandlordId { get; set; }
        [BindProperty] public string BasicEditName { get; set; }
        [BindProperty] public string BasicEditEmail { get; set; }
        [BindProperty] public string BasicEditPhone { get; set; }

        public async Task<IActionResult> OnPostEditBasicAsync()
        {
            var landlord = await _landlordService.GetLandlordAsync(BasicEditLandlordId);
            if (landlord == null) return NotFound();

            var dto = new LandlordUpdateDto
            {
                Name = BasicEditName,
                Email = BasicEditEmail,
                PhoneNumber = BasicEditPhone
            };

            await _landlordService.UpdateLandlordProfileAsync(BasicEditLandlordId, dto);
            return RedirectToPage();
        }

    }
}
