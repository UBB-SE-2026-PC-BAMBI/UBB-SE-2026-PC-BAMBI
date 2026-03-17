using BankApp.Models.DTOs.Auth;
namespace BankApp.Server.Services.Interfaces
{
    public interface IAuthService
    {
        LoginResponse Login(LoginRequest request);
        RegisterResponse Register(RegisterRequest request);
        LoginResponse VerifyOTP(VerifyOTPRequest request);
        void ResendOTP(int userId, string method);
        void RequestPasswordReset(string email);
        bool ResetPassword(string token, string newPassword);
    }
}