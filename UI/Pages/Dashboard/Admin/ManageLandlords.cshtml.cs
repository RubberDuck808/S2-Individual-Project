using BLL.DTOs.Landlord;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UI.Pages
{
    public class ManageLandlordsModel : PageModel
    {
        private readonly ILandlordService _landlordService;

        public ManageLandlordsModel(ILandlordService landlordService)
        {
            _landlordService = landlordService;
        }



        // List of landlords

        public List<LandlordDto> VerifiedLandlords { get; set; } = new();
        public List<LandlordDto> UnverifiedLandlords { get; set; } = new();

        public async Task OnGetAsync()
        {
            var all = (await _landlordService.GetAllAsync()).ToList();
            VerifiedLandlords = all.Where(l => l.IsVerified).ToList();
            UnverifiedLandlords = all.Where(l => !l.IsVerified).ToList();
        }

        // Verify
        [BindProperty]
        public int VerifyLandlordId { get; set; }

        [BindProperty]
        public bool VerifySetVerified { get; set; }

        public async Task<IActionResult> OnPostVerifyAsync()
        {
            var landlord = await _landlordService.GetLandlordAsync(VerifyLandlordId);
            if (landlord == null) return NotFound();

            var dto = new LandlordVerificationDto
            {
                IsVerified = VerifySetVerified,
                VerificationDate = VerifySetVerified ? DateTime.UtcNow : null,
                UpdatedAt = DateTime.UtcNow
            };

            await _landlordService.UpdateVerificationStatusAsync(VerifyLandlordId, dto);
            return RedirectToPage();
        }


        // Admin
        [BindProperty] public int AdminEditLandlordId { get; set; }
        [BindProperty] public string AdminEditFirstName { get; set; }
        [BindProperty] public string? AdminEditMiddleName { get; set; }
        [BindProperty] public string AdminEditLastName { get; set; }

        [BindProperty] public string AdminEditEmail { get; set; }
        [BindProperty] public string AdminEditPhone { get; set; }
        [BindProperty] public string? AdminEditCompany { get; set; }
        [BindProperty] public string? AdminEditTaxId { get; set; }

        public async Task<IActionResult> OnPostEditAdminAsync()
        {
            var landlord = await _landlordService.GetLandlordAsync(AdminEditLandlordId);
            if (landlord == null) return NotFound();

            var dto = new LandlordUpdateDto
            {
                FirstName = AdminEditFirstName,
                MiddleName = AdminEditMiddleName,
                LastName = AdminEditLastName,
                Email = AdminEditEmail,
                PhoneNumber = AdminEditPhone,
                CompanyName = AdminEditCompany,
                TaxIdentificationNumber = AdminEditTaxId
            };

            await _landlordService.UpdateLandlordProfileAsync(AdminEditLandlordId, dto);
            return RedirectToPage();
        }
    }
}
