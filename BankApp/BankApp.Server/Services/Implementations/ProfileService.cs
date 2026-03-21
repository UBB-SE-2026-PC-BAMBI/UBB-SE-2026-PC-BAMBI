using BankApp.Models.DTOs.Profile;
using BankApp.Models.Entities;
using BankApp.Models.Enums;
using BankApp.Server.Repositories;
using BankApp.Server.Repositories.Interfaces;
using BankApp.Server.Services.Interfaces;
using BankApp.Server.Utilities;

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

        public UpdateProfileResponse UpdatePersonalInfo(UpdateProfileRequest request)
        {
            User? user = _userRepository.FindById(request.UserId);

            if (user == null)
            {
                return new UpdateProfileResponse(false, "User not found.");
            }

            // Check and update phone number
            if (request.PhoneNumber != null)
            {
                if (!ValidationUtil.IsValidPhoneNumber(request.PhoneNumber))
                {
                    return new UpdateProfileResponse(false, "Invalid phone number.");
                }

                user.PhoneNumber = request.PhoneNumber;
            }

            // Check and update address
            if (request.Address != null)
            {
                user.Address = request.Address;
            }

            // Update the user in the repo
            if (_userRepository.UpdateUser(user) == false)
            {
                return new UpdateProfileResponse(false, "Could not update user.");
            }

            return new UpdateProfileResponse(true, "User profile updated successfully.");
        }

        public bool ChangePassword(int userId, string currentPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public bool Enable2FA(int userId, TwoFactorMethod method)
        {
            throw new NotImplementedException();
        }

        public bool Disable2FA(int userId, TwoFactorMethod method)
        {
            throw new NotImplementedException();
        }

        public List<OAuthLink> GetOAuthLinks(int userId)
        {
            throw new NotImplementedException();
        }

        public bool LinkOAuth(int userId, string provider)
        {
            throw new NotImplementedException();
        }

        public bool UnlinkOAuth(int userId, string provider)
        {
            throw new NotImplementedException();
        }

        public List<NotificationPreference> GetNotificationPreferences(int userId)
        {
            throw new NotImplementedException();
        }

        public bool UpdateNotificationPreferences(int userId, List<NotificationPreference> prefs)
        {
            throw new NotImplementedException();
        }
    }
}