using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BLL.Interfaces;
using BLL.DTOs.Landlord;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace UI.Pages.Dashboard.Landlord
{
    [Authorize(Roles = "Landlord")]
    public class EditAccountModel : PageModel
    {
        private readonly ILandlordService _landlordService;
        private readonly ILogger<EditAccountModel> _logger;

        public EditAccountModel(ILandlordService landlordService, ILogger<EditAccountModel> logger)
        {
            _landlordService = landlordService;
            _logger = logger;
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

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("OnGetAsync called for userId: {UserId}", userId);

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("UserId claim is missing. Redirecting to Login.");
                return RedirectToPage("/Login");
            }

            var landlord = await _landlordService.GetByUserIdAsync(userId);
            if (landlord == null)
            {
                _logger.LogWarning("No landlord found for userId {UserId}", userId);
                return NotFound();
            }

            // Fill properties
            LandlordEditId = landlord.LandlordId;
            LandlordEditFirstName = landlord.FirstName;
            LandlordEditMiddleName = landlord.MiddleName;
            LandlordEditLastName = landlord.LastName;
            LandlordEditEmail = landlord.Email;
            LandlordEditPhone = landlord.PhoneNumber;
            LandlordEditCompany = landlord.CompanyName;
            LandlordEditTaxNumber = landlord.TaxIdentificationNumber;
            SelectedLandlord = landlord;

            _logger.LogInformation("Loaded landlord data for userId {UserId}, landlordId {LandlordId}", userId, landlord.LandlordId);

            return Page();
        }


        public async Task<IActionResult> OnPostEditBasicAsync()
        {
            _logger.LogInformation("OnPostEditBasicAsync called for LandlordId: {LandlordId}", LandlordEditId);

            var landlord = await _landlordService.GetLandlordAsync(LandlordEditId);
            if (landlord == null)
            {
                _logger.LogWarning("Attempted to edit non-existent landlord with ID: {LandlordId}", LandlordEditId);
                return NotFound();
            }

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

            try
            {
                await _landlordService.UpdateLandlordProfileAsync(LandlordEditId, dto);
                _logger.LogInformation("Successfully updated landlord profile for ID: {LandlordId}", LandlordEditId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating landlord profile for ID: {LandlordId}", LandlordEditId);
                ModelState.AddModelError(string.Empty, "Failed to update landlord profile.");
                return Page();
            }

            return RedirectToPage();
        }


    }
}
