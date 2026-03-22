using BankApp.Client.Utilities;
using BankApp.Client.ViewModels;
using BankApp.Models.Entities;
using BankApp.Models.Enums;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.Generic;

namespace BankApp.Client.Views
{
    public sealed partial class ProfileView : Page, Observer<ProfileState>
    {
        private ProfileViewModel _viewModel;

        public ProfileView()
        {
            this.InitializeComponent();

            _viewModel = new ProfileViewModel(App.ApiService);
            _viewModel.State.AddObserver(this);
        }

        // ─── Navigation ────────────────────────────────────────────────────────────

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            /*
            if (e.Parameter is ProfileViewModel vm)
            {

                // If data already loaded before we got here
                if (_viewModel.ProfileInfo != null)
                    PopulateUI();
            }*/
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

            // Security tab
            //TwoFactorToggle.IsOn = user.Is2FAEnabled;
            TwoFactorToggle.IsOn = false; // TODO: replace with real value when API is ready
            TwoFactorPhoneBox.Text = user.PhoneNumber ?? string.Empty;

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
                var row = new Grid { Margin = new Thickness(0, 0, 0, 10) };
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                var border = new Border
                {
                    BorderBrush = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.LightGray),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(6),
                    Padding = new Thickness(10, 8, 10, 8),
                    Background = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.White)
                };

                var innerGrid = new Grid();
                innerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                innerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                innerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                var emailText = new TextBlock
                {
                    Text = link.ProviderEmail ?? link.Provider,
                    FontSize = 13,
                    Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(255, 30, 41, 59)),
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(emailText, 0);

                var removeBtn = new Button
                {
                    Style = (Style)Resources["GhostButtonStyle"],
                    Margin = new Thickness(0, 0, 0, 0),
                    Tag = link
                };
                removeBtn.Click += RemoveConnectedAccount_Click;
                var removeIcon = new FontIcon
                {
                    Glyph = "\xE711",
                    FontFamily = new Microsoft.UI.Xaml.Media.FontFamily("Segoe MDL2 Assets"),
                    FontSize = 13,
                    Foreground = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.ColorHelper.FromArgb(255, 239, 68, 68))
                };
                removeBtn.Content = removeIcon;
                Grid.SetColumn(removeBtn, 2);

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

        // ─── Password ───────────────────────────────────────────────────────────────

        private async void UpdatePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (NewPasswordBox.Password != ConfirmPasswordBox.Password)
            {
                ShowError("New passwords do not match.");
                return;
            }

            ShowLoading(true);

            bool success = await _viewModel.ChangePassword(
                CurrentPasswordBox.Password,
                NewPasswordBox.Password);

            ShowLoading(false);

            if (success)
            {
                CurrentPasswordBox.Password = string.Empty;
                NewPasswordBox.Password = string.Empty;
                ConfirmPasswordBox.Password = string.Empty;
                ShowSuccess("Password updated successfully.");
            }
            else
            {
                ShowError("Failed to change password. Check your current password and try again.");
            }
        }

        private void ToggleCurrentPassword_Click(object sender, RoutedEventArgs e) { /* toggle visibility logic */ }
        private void ToggleNewPassword_Click(object sender, RoutedEventArgs e) { /* toggle visibility logic */ }
        private void ToggleConfirmPassword_Click(object sender, RoutedEventArgs e) { /* toggle visibility logic */ }

        // ─── 2FA ────────────────────────────────────────────────────────────────────

        private async void TwoFactorToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (_viewModel == null) return;

            bool success = TwoFactorToggle.IsOn
                ? await _viewModel.EnableTwoFactor(BankApp.Models.Enums.TwoFactorMethod.Phone)
                : await _viewModel.DisableTwoFactor(BankApp.Models.Enums.TwoFactorMethod.Phone);

            if (!success)
            {
                // Revert toggle if the call failed
                TwoFactorToggle.Toggled -= TwoFactorToggle_Toggled;
                TwoFactorToggle.IsOn = !TwoFactorToggle.IsOn;
                TwoFactorToggle.Toggled += TwoFactorToggle_Toggled;
                ShowError("Failed to update 2FA settings.");
            }
        }

        private void SaveTwoFactorPhone_Click(object sender, RoutedEventArgs e) { /* save phone for 2FA */ }

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

        private void EditConnectedAccount_Click(object sender, RoutedEventArgs e) { /* edit flow */ }
        private void ManageDevicesButton_Click(object sender, RoutedEventArgs e) { /* navigate to sessions */ }

        // ─── Notifications ──────────────────────────────────────────────────────────

        private async void NotificationToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (_viewModel?.NotificationPreferences == null) return;

            if (sender is ToggleSwitch toggle && toggle.Tag is NotificationPreference pref)
                pref.PushEnabled = toggle.IsOn;

            await _viewModel.UpdateNotificationPreferences(_viewModel.NotificationPreferences);
        }

        // ─── Navigation ─────────────────────────────────────────────────────────────

        private void DashboardNavButton_Click(object sender, RoutedEventArgs e)
        {
            // nothing
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
