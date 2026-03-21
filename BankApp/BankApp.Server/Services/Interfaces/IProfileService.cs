using BankApp.Models.DTOs.Profile;
using BankApp.Models.Entities;
using BankApp.Models.Enums;
namespace BankApp.Server.Services.Interfaces
{
    public interface IProfileService
    {
        User? GetUserById(int userId);
        UpdateProfileResponse UpdatePersonalInfo(UpdateProfileRequest request);
        bool ChangePassword(int userId, string currentPassword, string newPassword);
        bool Enable2FA(int userId, TwoFactorMethod method);
        bool Disable2FA(int userId, TwoFactorMethod method);
        List<OAuthLink> GetOAuthLinks(int userId);
        bool LinkOAuth(int userId, string provider);
        bool UnlinkOAuth(int userId, string provider);
        List<NotificationPreference> GetNotificationPreferences(int userId);
        bool UpdateNotificationPreferences(int userId, List<NotificationPreference> prefs);
    }
}