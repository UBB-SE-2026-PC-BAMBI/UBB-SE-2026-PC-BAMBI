using BankApp.Client.Utilities;
using BankApp.Client.ViewModels;
using BankApp.Models.Entities;
using BankApp.Models.Enums;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;

namespace BankApp.Client.Views
{
    public sealed partial class ProfileView : Page, Observer<ProfileState>
    {
        private ProfileViewModel _viewModel;

        // Holds verified password
        private string _verifiedPassword = string.Empty;

        public ProfileView()
        {
            this.InitializeComponent();

            _viewModel = new ProfileViewModel(App.ApiService);
            _viewModel.State.AddObserver(this);
        }

        // ─── Navigation ─────────────────────────────────────────

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ShowLoading(true);

            await _viewModel.LoadProfile();

            ShowLoading(false);

            if (_viewModel.ProfileInfo != null)
                PopulateUI();

            SetEditingEnabled(false);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            _viewModel?.State.RemoveObserver(this);
        }

        // ─── UI Setup ─────────────────────────────────────────

        private void PopulateUI()
        {
            var user = _viewModel.ProfileInfo;

            ProfileCardName.Text = user.FullName ?? "";
            ProfileCardEmail.Text = user.Email ?? "";
            ProfileCardPhone.Text = user.PhoneNumber ?? "";
            ProfileCardAddress.Text = user.Address ?? "";

            FullNameBox.Text = user.FullName ?? "";
            EmailBox.Text = user.Email ?? "";

            PhoneBox.Text = user.PhoneNumber ?? "";
            AddressBox.Text = user.Address ?? "";

            TwoFactorPhoneBox.Text = user.PhoneNumber ?? "";
            TwoFactorEmailBox.Text = user.Email ?? "";

            PopulateOAuthLinks(_viewModel.OAuthLinks);
            PopulateNotificationPreferences(_viewModel.NotificationPreferences);
        }

        private void SetEditingEnabled(bool enabled)
        {
            PhoneBox.IsEnabled = enabled;
            AddressBox.IsEnabled = enabled;
            SaveButton.IsEnabled = enabled;
        }

        // ─── PERSONAL INFO FLOW ───────────────────────────────

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            VerifyCurrentPasswordBox.Password = "";
            VerifyErrorInfoBar.IsOpen = false;

            await VerifyPasswordDialog.ShowAsync().AsTask();
        }

        private async void VerifyPasswordDialog_PrimaryButtonClick(
            ContentDialog sender,
            ContentDialogButtonClickEventArgs args)
        {
            //var deferral = args.GetDeferral();

            //if (string.IsNullOrWhiteSpace(VerifyCurrentPasswordBox.Password))
            //{
            //    VerifyErrorInfoBar.Message = "Enter your password.";
            //    VerifyErrorInfoBar.IsOpen = true;
            //    args.Cancel = true;
            //    deferral.Complete();
            //    return;
            //}

            //bool verified = await _viewModel.VerifyPassword(VerifyCurrentPasswordBox.Password);

            //if (!verified)
            //{
            //    VerifyErrorInfoBar.Message = "Incorrect password.";
            //    VerifyErrorInfoBar.IsOpen = true;
            //    args.Cancel = true;
            //    deferral.Complete();
            //    return;
            //}

            //_verifiedPassword = VerifyCurrentPasswordBox.Password;

            //SetEditingEnabled(true);

            //VerifyErrorInfoBar.IsOpen = false;
            //deferral.Complete();

            //ShowSuccess("You can now edit your profile.");
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ShowLoading(true);

            bool success = await _viewModel.UpdatePersonalInfo(
                PhoneBox.Text,
                AddressBox.Text,
                _verifiedPassword);

            ShowLoading(false);

            if (success)
            {
                ProfileCardPhone.Text = PhoneBox.Text.Trim();
                ProfileCardAddress.Text = AddressBox.Text.Trim();

                _verifiedPassword = "";
                SetEditingEnabled(false);

                ShowSuccess("Profile updated successfully.");
            }
            else
            {
                ShowError("Failed to update profile.");
            }
        }

        // ─── PASSWORD CHANGE ───────────────────────────────

        private async void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            VerifyCurrentPasswordBox.Password = "";
            VerifyErrorInfoBar.IsOpen = false;

            await VerifyPasswordDialog.ShowAsync().AsTask();
        }

        private async void NewPasswordDialog_PrimaryButtonClick(
            ContentDialog sender,
            ContentDialogButtonClickEventArgs args)
        {
            var deferral = args.GetDeferral();

            if (NewPasswordBox.Password.Length < 8)
            {
                NewPasswordErrorInfoBar.Message = "Minimum 8 characters.";
                NewPasswordErrorInfoBar.IsOpen = true;
                args.Cancel = true;
                deferral.Complete();
                return;
            }

            if (NewPasswordBox.Password != ConfirmPasswordBox.Password)
            {
                NewPasswordErrorInfoBar.Message = "Passwords do not match.";
                NewPasswordErrorInfoBar.IsOpen = true;
                args.Cancel = true;
                deferral.Complete();
                return;
            }

            bool success = await _viewModel.ChangePassword(
                _verifiedPassword,
                NewPasswordBox.Password);

            if (success)
            {
                _verifiedPassword = "";
                NewPasswordErrorInfoBar.IsOpen = false;

                deferral.Complete();
                ShowSuccess("Password updated.");
            }
            else
            {
                NewPasswordErrorInfoBar.Message = "Failed to update password.";
                NewPasswordErrorInfoBar.IsOpen = true;
                args.Cancel = true;
                deferral.Complete();
            }
        }

        // ─── 2FA ─────────────────────────────────────────
        private async void SaveTwoFactorSettings_Click(object sender, RoutedEventArgs e)
        {
            //bool success = await _viewModel.UpdateTwoFactorContacts(
            //    TwoFactorPhoneBox.Text.Trim(),
            //    TwoFactorEmailBox.Text.Trim());

            //if (success)
            //    ShowSuccess("2FA settings saved.");
            //else
            //    ShowError("Failed to save 2FA settings.");
        }
        private async void TwoFactorToggle_Toggled(object sender, RoutedEventArgs e)
        {
            bool success = TwoFactorToggle.IsOn
                ? await _viewModel.EnableTwoFactor(TwoFactorMethod.Phone)
                : await _viewModel.DisableTwoFactor(TwoFactorMethod.Phone);

            if (!success)
                ShowError("2FA update failed.");
        }

        private async void TwoFactorEmailToggle_Toggled(object sender, RoutedEventArgs e)
        {
            bool success = TwoFactorEmailToggle.IsOn
                ? await _viewModel.EnableTwoFactor(TwoFactorMethod.Email)
                : await _viewModel.DisableTwoFactor(TwoFactorMethod.Email);

            if (!success)
                ShowError("2FA email update failed.");
        }

        // ─── OAuth ─────────────────────────────────────────

        private async void RemoveConnectedAccount_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is OAuthLink link)
            {
                bool success = await _viewModel.UnlinkOAuth(link.Provider);

                if (success)
                    PopulateOAuthLinks(_viewModel.OAuthLinks);
                else
                    ShowError("Failed to remove account.");
            }
        }

        private void ManageDevicesButton_Click(object sender, RoutedEventArgs e)
        {
        }

        // ─── Notifications ─────────────────────────

        private async void NotificationToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggle && toggle.Tag is NotificationPreference pref)
                pref.PushEnabled = toggle.IsOn;

            await _viewModel.UpdateNotificationPreferences(_viewModel.NotificationPreferences);
        }

        // ─── Navigation ─────────────────────────

        private void DashboardNavButton_Click(object sender, RoutedEventArgs e)
        {
            App.NavigationService.NavigateTo<DashboardView>();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            App.NavigationService.NavigateTo<LoginView>();
        }

        // ─── Helpers ─────────────────────────

        private void ShowLoading(bool visible)
        {
            LoadingPanel.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            LoadingRing.IsActive = visible;
            ErrorInfoBar.IsOpen = false;
            SuccessInfoBar.IsOpen = false;
        }

        private void ShowError(string message)
        {
            ErrorInfoBar.Message = message;
            ErrorInfoBar.IsOpen = true;
            SuccessInfoBar.IsOpen = false;
        }

        private void ShowSuccess(string message)
        {
            SuccessInfoBar.Message = message;
            SuccessInfoBar.IsOpen = true;
            ErrorInfoBar.IsOpen = false;
        }

        // ─── Observer ─────────────────────────

        public void Update(ProfileState state)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                switch (state)
                {
                    case ProfileState.Loading:
                        ShowLoading(true);
                        break;

                    case ProfileState.UpdateSuccess:
                        ShowLoading(false);
                        PopulateUI();
                        break;

                    case ProfileState.Error:
                        ShowLoading(false);
                        ShowError("Operation failed.");
                        break;
                }
            });
        }

        // ─── EXISTING HELPERS (unchanged) ─────────────────────────

        // ─── TAB SWITCHING ─────────────────────────

        private void TabPersonalBtn_Click(object sender, RoutedEventArgs e)
        {
            PanelPersonal.Visibility = Visibility.Visible;
            PanelSecurity.Visibility = Visibility.Collapsed;
            PanelNotifications.Visibility = Visibility.Collapsed;

            TabPersonalBtn.Style = (Style)Resources["TabButtonActiveStyle"];
            TabSecurityBtn.Style = (Style)Resources["TabButtonStyle"];
            TabNotificationsBtn.Style = (Style)Resources["TabButtonStyle"];
        }

        private void TabSecurityBtn_Click(object sender, RoutedEventArgs e)
        {
            PanelPersonal.Visibility = Visibility.Collapsed;
            PanelSecurity.Visibility = Visibility.Visible;
            PanelNotifications.Visibility = Visibility.Collapsed;

            TabPersonalBtn.Style = (Style)Resources["TabButtonStyle"];
            TabSecurityBtn.Style = (Style)Resources["TabButtonActiveStyle"];
            TabNotificationsBtn.Style = (Style)Resources["TabButtonStyle"];
        }

        private void TabNotificationsBtn_Click(object sender, RoutedEventArgs e)
        {
            PanelPersonal.Visibility = Visibility.Collapsed;
            PanelSecurity.Visibility = Visibility.Collapsed;
            PanelNotifications.Visibility = Visibility.Visible;

            TabPersonalBtn.Style = (Style)Resources["TabButtonStyle"];
            TabSecurityBtn.Style = (Style)Resources["TabButtonStyle"];
            TabNotificationsBtn.Style = (Style)Resources["TabButtonActiveStyle"];
        }

        private void PopulateOAuthLinks(List<OAuthLink> links)
        {
            OAuthLinksPanel.Children.Clear();

            if (links == null) return;

            foreach (var link in links)
            {
                var btn = new Button
                {
                    Content = link.ProviderEmail ?? link.Provider,
                    Tag = link
                };

                btn.Click += RemoveConnectedAccount_Click;
                OAuthLinksPanel.Children.Add(btn);
            }
        }

        private void PopulateNotificationPreferences(List<NotificationPreference> prefs)
        {
            NotificationPreferencesPanel.Children.Clear();

            if (prefs == null) return;

            foreach (var pref in prefs)
            {
                var toggle = new ToggleSwitch
                {
                    IsOn = pref.PushEnabled,
                    Tag = pref
                };

                toggle.Toggled += NotificationToggle_Toggled;
                NotificationPreferencesPanel.Children.Add(toggle);
            }
        }
    }
}