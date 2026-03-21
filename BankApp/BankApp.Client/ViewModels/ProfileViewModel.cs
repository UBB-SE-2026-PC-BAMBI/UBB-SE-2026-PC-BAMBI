using BankApp.Client.Utilities;
using BankApp.Client.ViewModels.Base;
using BankApp.Models.DTOs.Profile;
using BankApp.Models.Entities;
using BankApp.Models.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BankApp.Client.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private readonly ApiService _apiService;
        private bool _disposed;

        public Observable<ProfileState> State { get; private set; }
        public User CurrentUser { get; private set; }
        public List<OAuthLink> OAuthLinks { get; private set; }
        public List<Session> ActiveSessions { get; private set; }
        public List<NotificationPreference> NotificationPreferences { get; private set; }

        public ProfileViewModel(
            ApiService apiService,
            Observable<ProfileState> state,
            User currentUser,
            List<OAuthLink> oAuthLinks,
            List<Session> activeSessions,
            List<NotificationPreference> notificationPreferences)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            State = state ?? throw new ArgumentNullException(nameof(state));
            CurrentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            OAuthLinks = oAuthLinks ?? new List<OAuthLink>();
            ActiveSessions = activeSessions ?? new List<Session>();
            NotificationPreferences = notificationPreferences ?? new List<NotificationPreference>();
        }

        public async Task<bool> UpdatePersonalInfo(string phone, string address, string password)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phone) && string.IsNullOrWhiteSpace(address))
                    return false;

                State.SetValue(ProfileState.Loading);

                var request = new UpdateProfileRequest
                {
                    PhoneNumber = phone?.Trim(),
                    Address = address?.Trim()
                };

                var result = await _apiService.PostAsync<UpdateProfileRequest, bool>(
                    $"api/profile/{CurrentUser.Id}", request);

                if (result)
                {
                    if (!string.IsNullOrWhiteSpace(phone)) CurrentUser.PhoneNumber = phone.Trim();
                    if (!string.IsNullOrWhiteSpace(address)) CurrentUser.Address = address.Trim();
                    State.SetValue(ProfileState.UpdateSuccess);
                }
                else
                {
                    State.SetValue(ProfileState.Error);
                }

                return result;
            }
            catch (Exception ex)
            {
                State.SetValue(ProfileState.Error);
                LogError(nameof(UpdatePersonalInfo), ex);
                return false;
            }
        }

     
        public async Task<bool> ChangePassword(string currentPassword, string newPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(currentPassword) ||
                    string.IsNullOrWhiteSpace(newPassword))
                    return false;

                if (currentPassword == newPassword)
                    return false;

                State.SetValue(ProfileState.Loading);

                var request = new ChangePasswordRequest
                {
                    CurrentPassword = currentPassword,
                    NewPassword = newPassword
                };

                var result = await _apiService.PostAsync<ChangePasswordRequest, bool>(
                    $"api/profile/{CurrentUser.Id}/password", request);

                State.SetValue(result ? ProfileState.UpdateSuccess : ProfileState.Error);
                return result;
            }
            catch (Exception ex)
            {
                State.SetValue(ProfileState.Error);
                LogError(nameof(ChangePassword), ex);
                return false;
            }
        }
        public async Task<bool> EnableTwoFactor(TwoFactorMethod method)
        {
            try
            {
                State.SetValue(ProfileState.Loading);

                var request = new { Method = method.ToString() };

                var result = await _apiService.PostAsync<object, bool>(
                    $"api/profile/{CurrentUser.Id}/2fa/enable", request);

                if (result)
                {
                    CurrentUser.Is2FAEnabled = true;
                    CurrentUser.Preferred2FAMethod = method.ToString();
                    State.SetValue(ProfileState.UpdateSuccess);
                }
                else
                {
                    State.SetValue(ProfileState.Error);
                }

                return result;
            }
            catch (Exception ex)
            {
                State.SetValue(ProfileState.Error);
                LogError(nameof(EnableTwoFactor), ex);
                return false;
            }
        }

        public async Task<bool> DisableTwoFactor(TwoFactorMethod method)
        {
            try
            {
                if (!CurrentUser.Is2FAEnabled)
                    return false;

                State.SetValue(ProfileState.Loading);

                var request = new { Method = method.ToString() };

                var result = await _apiService.PostAsync<object, bool>(
                    $"api/profile/{CurrentUser.Id}/2fa/disable", request);

                if (result)
                {
                    CurrentUser.Is2FAEnabled = false;
                    CurrentUser.Preferred2FAMethod = null;
                    State.SetValue(ProfileState.UpdateSuccess);
                }
                else
                {
                    State.SetValue(ProfileState.Error);
                }

                return result;
            }
            catch (Exception ex)
            {
                State.SetValue(ProfileState.Error);
                LogError(nameof(DisableTwoFactor), ex);
                return false;
            }
        }

       
        public async Task<bool> LinkOAuth(string provider)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(provider))
                    return false;

                var alreadyLinked = OAuthLinks.Exists(o =>
                    string.Equals(o.Provider, provider, StringComparison.OrdinalIgnoreCase));

                if (alreadyLinked)
                    return false;

                State.SetValue(ProfileState.Loading);

                var request = new { Provider = provider.Trim() };

                var result = await _apiService.PostAsync<object, bool>(
                    $"api/profile/{CurrentUser.Id}/oauth/link", request);

                if (result)
                {
                    OAuthLinks.Add(new OAuthLink { Provider = provider, UserId = CurrentUser.Id });
                    State.SetValue(ProfileState.UpdateSuccess);
                }
                else
                {
                    State.SetValue(ProfileState.Error);
                }

                return result;
            }
            catch (Exception ex)
            {
                State.SetValue(ProfileState.Error);
                LogError(nameof(LinkOAuth), ex);
                return false;
            }
        }

        
        public async Task<bool> UnlinkOAuth(string provider)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(provider))
                    return false;

                var existing = OAuthLinks.Find(o =>
                    string.Equals(o.Provider, provider, StringComparison.OrdinalIgnoreCase));

                if (existing == null)
                    return false;

                State.SetValue(ProfileState.Loading);

                var request = new { Provider = provider.Trim() };

                var result = await _apiService.PostAsync<object, bool>(
                    $"api/profile/{CurrentUser.Id}/oauth/unlink", request);

                if (result)
                {
                    OAuthLinks.Remove(existing);
                    State.SetValue(ProfileState.UpdateSuccess);
                }
                else
                {
                    State.SetValue(ProfileState.Error);
                }

                return result;
            }
            catch (Exception ex)
            {
                State.SetValue(ProfileState.Error);
                LogError(nameof(UnlinkOAuth), ex);
                return false;
            }
        }

        
        public async Task<bool> UpdateNotificationPreferences(List<NotificationPreference> preferences)
        {
            try
            {
                if (preferences == null || preferences.Count == 0)
                    return false;

                State.SetValue(ProfileState.Loading);

                var result = await _apiService.PostAsync<List<NotificationPreference>, bool>(
                    $"api/profile/{CurrentUser.Id}/notifications/preferences", preferences);

                if (result)
                {
                    NotificationPreferences = preferences;
                    State.SetValue(ProfileState.UpdateSuccess);
                }
                else
                {
                    State.SetValue(ProfileState.Error);
                }

                return result;
            }
            catch (Exception ex)
            {
                State.SetValue(ProfileState.Error);
                LogError(nameof(UpdateNotificationPreferences), ex);
                return false;
            }
        }

        public override void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        private void LogError(string method, Exception ex) =>
            Console.Error.WriteLine($"[ProfileViewModel] {method}: {ex.Message}");
    }
}