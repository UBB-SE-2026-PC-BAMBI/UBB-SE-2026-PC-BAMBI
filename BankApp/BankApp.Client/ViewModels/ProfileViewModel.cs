using BankApp.Client.Utilities;
using BankApp.Client.ViewModels.Base;
using BankApp.Models.Entities;
using BankApp.Models.Enums;
using System.Collections.Generic;
using System;

namespace BankApp.Client.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        public Observable<ProfileState> State { get; private set; }
        public Observable<User> CurrentUser { get; private set; }
        public Observable<List<OAuthLink>> OAuthLinks { get; private set; }
        public Observable<List<Session>> ActiveSessions { get; private set; }
        public Observable<List<NotificationPreference>> NotificationPreferences { get; private set; }

        public bool UpdatePersonalInfo(string phone, string address, string password)
        {
            throw new NotImplementedException();
        }

        public bool ChangePassword(string currentPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public bool EnableTwoFactor(TwoFactorMethod method)
        {
            throw new NotImplementedException();
        }

        public bool DisableTwoFactor(TwoFactorMethod method)
        {
            throw new NotImplementedException();
        }

        public bool LinkOAuth(string provider)
        {
            throw new NotImplementedException();
        }

        public bool UnlinkOAuth(string provider)
        {
            throw new NotImplementedException();
        }

        public bool UpdateNotificationPreferences(List<NotificationPreference> preferences)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    } 
}