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
using Windows.Foundation; // required for IAsyncOperation<ContentDialogResult>.AsTask()

namespace BankApp.Client.Views
{
    public sealed partial class ProfileView : Page, Observer<ProfileState>
    {
        private ProfileViewModel _viewModel;

        // Holds the verified current password between Step 1 and Step 2
        // so Step 2 can pass it to ChangePassword() without re-asking.
        private string _verifiedCurrentPassword = string.Empty;

        public ProfileView()
        {
            this.InitializeComponent();

            _viewModel = new ProfileViewModel(App.ApiService);
            _viewModel.State.AddObserver(this);
        }

        // ─── Navigation ────────────────────────────────────────────────────────────

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ShowLoading(true);

           await _viewModel.LoadProfile();

            ShowLoading(false);

            if (_viewModel.ProfileInfo != null)
                PopulateUI();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            _viewModel?.State.RemoveObserver(this);
        }

        // ─── Population ─────────────────────────────────────────────────────────────

        private void PopulateUI()
        {
            var user = _viewModel.ProfileInfo;

            // Profile card
            ProfileCardName.Text = user.FullName ?? string.Empty;
            ProfileCardEmail.Text = user.Email ?? string.Empty;
            ProfileCardPhone.Text = user.PhoneNumber ?? string.Empty;
            ProfileCardAddress.Text = user.Address ?? string.Empty;

            // Personal info tab — read-only fields
            FullNameBox.Text = user.FullName ?? string.Empty;
            EmailBox.Text = user.Email ?? string.Empty;

            // Personal info tab — editable fields
            PhoneBox.Text = user.PhoneNumber ?? string.Empty;
            AddressBox.Text = user.Address ?? string.Empty;

            // Security tab — 2FA
            TwoFactorToggle.IsOn = false; // TODO: replace with real value when API is ready
            TwoFactorPhoneBox.Text = user.PhoneNumber ?? string.Empty;
            TwoFactorEmailBox.Text = user.Email ?? string.Empty;

            // OAuth links
            PopulateOAuthLinks(_viewModel.OAuthLinks);

            // Notification preferences
            PopulateNotificationPreferences(_viewModel.NotificationPreferences);
        }

        private void PopulateOAuthLinks(List<OAuthLink> links)
        {
            OAuthLinksPanel.Children.Clear();

            if (links == null || links.Count == 0)
                return;

            foreach (var link in links)
            {
                var border = new Border
                {
                    BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.LightGray),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(6),
                    Padding = new Thickness(10, 8, 10, 8),
                    Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White),
                    Margin = new Thickness(0, 0, 0, 10)
                };

                var innerGrid = new Grid();
                innerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                innerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                var emailText = new TextBlock
                {
                    Text = link.ProviderEmail ?? link.Provider,
                    FontSize = 13,
                    Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(255, 30, 41, 59)),
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(emailText, 0);

                var removeIcon = new FontIcon
                {
                    Glyph = "\xE711",
                    FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Segoe MDL2 Assets"),
                    FontSize = 13,
                    Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(255, 239, 68, 68))
                };

                var removeBtn = new Button
                {
                    Content = removeIcon,
                    Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent),
                    BorderThickness = new Thickness(0),
                    Padding = new Thickness(6),
                    Tag = link
                };
                removeBtn.Click += RemoveConnectedAccount_Click;
                Grid.SetColumn(removeBtn, 1);

                innerGrid.Children.Add(emailText);
                innerGrid.Children.Add(removeBtn);
                border.Child = innerGrid;

                OAuthLinksPanel.Children.Add(border);
            }
        }

        private void PopulateNotificationPreferences(List<NotificationPreference> prefs)
        {
            NotificationPreferencesPanel.Children.Clear();

            if (prefs == null || prefs.Count == 0)
                return;

            for (int i = 0; i < prefs.Count; i++)
            {
                var pref = prefs[i];
                bool isLast = i == prefs.Count - 1;

                var border = new Border
                {
                    BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(255, 226, 232, 240)),
                    BorderThickness = isLast ? new Thickness(0) : new Thickness(0, 0, 0, 1),
                    Padding = new Thickness(0, 14, 0, 14)
                };

                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                var labelStack = new StackPanel { Orientation = Orientation.Vertical };

                var title = new TextBlock
                {
                    Text = pref.Category ?? string.Empty,
                    FontSize = 13.5,
                    FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Segoe UI Semibold"),
                    Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(255, 30, 41, 59))
                };

                labelStack.Children.Add(title);
                Grid.SetColumn(labelStack, 0);

                var toggle = new ToggleSwitch
                {
                    IsOn = pref.PushEnabled,
                    OnContent = string.Empty,
                    OffContent = string.Empty,
                    VerticalAlignment = VerticalAlignment.Center,
                    Tag = pref
                };
                toggle.Toggled += NotificationToggle_Toggled;
                Grid.SetColumn(toggle, 1);

                grid.Children.Add(labelStack);
                grid.Children.Add(toggle);
                border.Child = grid;

                NotificationPreferencesPanel.Children.Add(border);
            }
        }

        // ─── Tab switching ──────────────────────────────────────────────────────────

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

        // ─── Personal info ──────────────────────────────────────────────────────────

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ShowLoading(true);

            bool success = await _viewModel.UpdatePersonalInfo(
                PhoneBox.Text,
                AddressBox.Text,
                password: string.Empty);

            ShowLoading(false);

            if (success)
            {
                ProfileCardPhone.Text = PhoneBox.Text.Trim();
                ProfileCardAddress.Text = AddressBox.Text.Trim();
                ShowSuccess("Profile updated successfully.");
            }
            else
            {
                ShowError("Failed to update profile. Please try again.");
            }
        }

        // ─── Password — two-step flow ────────────────────────────────────────────────
        //
        //  Step 1  VerifyPasswordDialog   — user enters current password only.
        //          On success the dialog closes and Step 2 opens automatically.
        //
        //  Step 2  NewPasswordDialog      — user enters new + confirm password.
        //          Uses _verifiedCurrentPassword stored from Step 1.

        private async void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            // Reset Step 1
            VerifyCurrentPasswordBox.Password = string.Empty;
            VerifyErrorInfoBar.IsOpen = false;

            await VerifyPasswordDialog.ShowAsync().AsTask();
            // Step 2 is triggered from inside VerifyPasswordDialog_PrimaryButtonClick on success.
        }

        /// <summary>
        /// Step 1 — verifies the current password via the API.
        /// Keeps the dialog open on failure; closes it and opens Step 2 on success.
        /// </summary>
        private async void VerifyPasswordDialog_PrimaryButtonClick(
            ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //var deferral = args.GetDeferral();

            //if (string.IsNullOrWhiteSpace(VerifyCurrentPasswordBox.Password))
            //{
            //    VerifyErrorInfoBar.Message = "Please enter your current password.";
            //    VerifyErrorInfoBar.IsOpen = true;
            //    args.Cancel = true;
            //    deferral.Complete();
            //    return;
            //}

            //bool verified = await _viewModel.VerifyPassword(VerifyCurrentPasswordBox.Password);

            //if (!verified)
            //{
            //    VerifyErrorInfoBar.Message = "Incorrect password. Please try again.";
            //    VerifyErrorInfoBar.IsOpen = true;
            //    args.Cancel = true;
            //    deferral.Complete();
            //    return;
            //}

            //// Stash the verified password and close Step 1.
            //_verifiedCurrentPassword = VerifyCurrentPasswordBox.Password;
            //VerifyErrorInfoBar.IsOpen = false;
            //deferral.Complete();

            //// Open Step 2 after Step 1 has fully dismissed.
            //NewPasswordBox.Password = string.Empty;
            //ConfirmPasswordBox.Password = string.Empty;
            //NewPasswordErrorInfoBar.IsOpen = false;

            //await NewPasswordDialog.ShowAsync().AsTask();
        }

        /// <summary>
        /// Step 2 — validates new password and saves it.
        /// Keeps the dialog open on failure; closes it and shows a success bar on success.
        /// </summary>
        private async void NewPasswordDialog_PrimaryButtonClick(
            ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var deferral = args.GetDeferral();

            if (string.IsNullOrWhiteSpace(NewPasswordBox.Password) ||
                NewPasswordBox.Password.Length < 8)
            {
                NewPasswordErrorInfoBar.Message = "New password must be at least 8 characters.";
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
                _verifiedCurrentPassword,
                NewPasswordBox.Password);

            if (success)
            {
                _verifiedCurrentPassword = string.Empty; // clear sensitive data
                NewPasswordErrorInfoBar.IsOpen = false;
                deferral.Complete();
                ShowSuccess("Password updated successfully.");
            }
            else
            {
                NewPasswordErrorInfoBar.Message = "Failed to update password. Please try again.";
                NewPasswordErrorInfoBar.IsOpen = true;
                args.Cancel = true;
                deferral.Complete();
            }
        }

        // ─── 2FA ────────────────────────────────────────────────────────────────────

        private async void TwoFactorToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null) return;

            bool success = TwoFactorToggle.IsOn
                ? await _viewModel.EnableTwoFactor(TwoFactorMethod.Phone)
                : await _viewModel.DisableTwoFactor(TwoFactorMethod.Phone);

            if (!success)
            {
                TwoFactorToggle.Toggled -= TwoFactorToggle_Toggled;
                TwoFactorToggle.IsOn = !TwoFactorToggle.IsOn;
                TwoFactorToggle.Toggled += TwoFactorToggle_Toggled;
                ShowError("Failed to update phone 2FA setting.");
            }
        }

        private async void TwoFactorEmailToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null) return;

            bool success = TwoFactorEmailToggle.IsOn
                ? await _viewModel.EnableTwoFactor(TwoFactorMethod.Email)
                : await _viewModel.DisableTwoFactor(TwoFactorMethod.Email);

            if (!success)
            {
                TwoFactorEmailToggle.Toggled -= TwoFactorEmailToggle_Toggled;
                TwoFactorEmailToggle.IsOn = !TwoFactorEmailToggle.IsOn;
                TwoFactorEmailToggle.Toggled += TwoFactorEmailToggle_Toggled;
                ShowError("Failed to update email 2FA setting.");
            }
        }

        /// <summary>
        /// Saves both the 2FA phone number and email address.
        /// </summary>
        private async void SaveTwoFactorSettings_Click(object sender, RoutedEventArgs e)
        {
            //ShowLoading(true);

            //bool success = await _viewModel.UpdateTwoFactorContacts(
            //    TwoFactorPhoneBox.Text.Trim(),
            //    TwoFactorEmailBox.Text.Trim());

            //ShowLoading(false);

            //if (success)
            //    ShowSuccess("2FA settings saved.");
            //else
            //    ShowError("Failed to save 2FA settings. Please try again.");
        }

        // ─── OAuth ──────────────────────────────────────────────────────────────────

        private async void RemoveConnectedAccount_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is OAuthLink link)
            {
                bool success = await _viewModel.UnlinkOAuth(link.Provider);
                if (success)
                    PopulateOAuthLinks(_viewModel.OAuthLinks);
                else
                    ShowError("Failed to remove connected account.");
            }
        }

        private void ManageDevicesButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: navigate to connected-accounts / sessions management view
        }

        // ─── Notifications ──────────────────────────────────────────────────────────

        private async void NotificationToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (_viewModel?.NotificationPreferences == null) return;

            if (sender is ToggleSwitch toggle && toggle.Tag is NotificationPreference pref)
                pref.PushEnabled = toggle.IsOn;

            await _viewModel.UpdateNotificationPreferences(_viewModel.NotificationPreferences);
        }

        // ─── Sidebar navigation ─────────────────────────────────────────────────────

        private void DashboardNavButton_Click(object sender, RoutedEventArgs e)
        {
            App.NavigationService.NavigateTo<DashboardView>();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            App.NavigationService.NavigateTo<LoginView>();
        }

        // ─── UI helpers ─────────────────────────────────────────────────────────────

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

        // ─── Observer ───────────────────────────────────────────────────────────────

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
                        if (_viewModel.ProfileInfo != null)
                            PopulateUI();
                        break;

                    case ProfileState.Error:
                        ShowLoading(false);
                        ShowError("Failed to load profile.");
                        break;
                }
            });
        }
    }
}
