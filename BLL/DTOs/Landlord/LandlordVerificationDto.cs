namespace BLL.DTOs.Landlord
{
    public class LandlordVerificationDto
    {
        public bool IsVerified { get; set; }
        public DateTime? VerificationDate { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}
