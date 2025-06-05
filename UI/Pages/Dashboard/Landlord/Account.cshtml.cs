using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BLL.Interfaces;
using BLL.DTOs.Landlord;

namespace UI.Pages.Dashboard.Landlord
{
    public class AccountModel : PageModel
    {
        private readonly ILandlordService _landlordService;

        public AccountModel(ILandlordService landlordService)
        {
            _landlordService = landlordService;
        }

        public List<LandlordDto> VerifiedLandlords { get; set; } = new();
        public List<LandlordDto> UnverifiedLandlords { get; set; } = new();
        public LandlordDto? SelectedLandlord { get; set; }

        [BindProperty] public int LandlordEditId { get; set; }
        [BindProperty] public string LandlordEditFirstName { get; set; } = string.Empty;
        [BindProperty] public string? LandlordEditMiddleName { get; set; }
        [BindProperty] public string LandlordEditLastName { get; set; } = string.Empty;
        [BindProperty] public string LandlordEditEmail { get; set; } = string.Empty;
        [BindProperty] public string LandlordEditPhone { get; set; } = string.Empty;
        [BindProperty] public string? LandlordEditCompany { get; set; }
        [BindProperty] public string? LandlordEditTaxNumber { get; set; }

        public async Task OnGetAsync()
        {
            var all = (await _landlordService.GetAllAsync()).ToList();
            VerifiedLandlords = all.Where(l => l.IsVerified).ToList();
            UnverifiedLandlords = all.Where(l => !l.IsVerified).ToList();
        }

        public async Task<IActionResult> OnPostEditBasicAsync()
        {
            SelectedLandlord = await _landlordService.GetLandlordAsync(LandlordEditId);
            if (SelectedLandlord == null) return NotFound();

            var dto = new LandlordUpdateDto
            {
                FirstName = LandlordEditFirstName,
                MiddleName = LandlordEditMiddleName,
                LastName = LandlordEditLastName,
                Email = LandlordEditEmail,
                PhoneNumber = LandlordEditPhone,
                CompanyName = LandlordEditCompany,
                TaxIdentificationNumber = LandlordEditTaxNumber
            };

            await _landlordService.UpdateLandlordProfileAsync(LandlordEditId, dto);
            return RedirectToPage();
        }
    }
}
