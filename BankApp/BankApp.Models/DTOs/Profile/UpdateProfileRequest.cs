namespace BankApp.Models.DTOs.Profile
{
    public class UpdateProfileRequest
    {
        public int UserId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
    }
}
