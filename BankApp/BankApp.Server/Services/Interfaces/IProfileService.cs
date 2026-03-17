using BankApp.Models.Entities;
using BankApp.Models.Enums;
namespace BankApp.Server.Services.Interfaces
{
    public interface IProfileService
    {
        User? GetUserById(int userId);
        bool UpdatePersonalInfo(int userId, string? phone, string? address);
        bool ChangePassword(int userId, string currentPassword, string newPassword);
        bool Enable2FA(int userId, TwoFactorMethod method);
        bool Disable2FA(int userId, TwoFactorMethod method);
        List<OAuthLink> GetOAuthLinks(int userId);
        List<NotificationPreference> GetNotificationPreferences(int userId);
        bool UpdateNotificationPreferences(int userId, List<NotificationPreference> prefs);
    }
}