using BankApp.Models.Entities;
using BankApp.Models.Enums;
using BankApp.Server.Repositories;
using BankApp.Server.Repositories.Interfaces;
using BankApp.Server.Services.Interfaces;

namespace BankApp.Server.Services.Implementations
{
    public class ProfileService : IProfileService
    {
        private readonly IUserRepository _userRepository;

        public ProfileService()
        {
            _userRepository = new UserRepository();
        }

        public User? GetUserById(int userId)
        {
            return _userRepository.FindById(userId);
        }

        public bool UpdatePersonalInfo(int userId, string? phone, string? address)
        {
            User? user = _userRepository.FindById(userId);

            if (user == null)
            {
                return false;
            }

            if (phone != null) {
        }

        public bool ChangePassword(int userId, string currentPassword, string newPassword)
        {
        }

        public bool Enable2FA(int userId, TwoFactorMethod method)
        {
        }

        public bool Disable2FA(int userId, TwoFactorMethod method)
        {
        }

        public List<OAuthLink> GetOAuthLinks(int userId)
        {
        }

        public bool LinkOAuth(int userId, string provider)
        {
        }

        public bool UnlinkOAuth(int userId, string provider)
        {
        }

        public List<NotificationPreference> GetNotificationPreferences(int userId)
        {
        }

        public bool UpdateNotificationPreferences(int userId, List<NotificationPreference> prefs)
        {
        }
    }
}