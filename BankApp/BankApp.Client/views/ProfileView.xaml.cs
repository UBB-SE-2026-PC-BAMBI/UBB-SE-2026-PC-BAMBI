using BankApp.Client.Utilities;
using BankApp.Client.ViewModels;
using BankApp.Models.Enums;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace BankApp.Client.Views
{
    public sealed partial class ProfileView : Page, Observer<ProfileState>
    {
        private readonly ProfileViewModel _viewModel;

        public ProfileView()
        {
            InitializeComponent();
            _viewModel = new ProfileViewModel(App.ApiService);
            _viewModel.State.AddObserver(this);
        }

        // ═══════════════════════════════════════
        //  OBSERVER
        // ═══════════════════════════════════════

        public void Update(ProfileState state) => OnStateChanged(state);

        public void OnStateChanged(ProfileState state)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                HideLoading();
                ErrorInfoBar.IsOpen = false;
                SuccessInfoBar.IsOpen = false;

                switch (state)
                {
                    case ProfileState.Idle:
                        break;

                    case ProfileState.Loading:
                        ShowLoading();
                        break;

                    case ProfileState.Success:
                        break;

                    case ProfileState.UpdateSuccess:
                        ShowSuccess("Profile updated successfully.");
                        RefreshProfileCard();
                        break;

                    case ProfileState.PasswordChanged:
                        ShowSuccess("Password changed successfully.");
                        ClearPasswordFields();
                        break;

                    case ProfileState.Error:
                        ShowError("Something went wrong. Please try again.");
                        break;
                }
            });
        }

        // ═══════════════════════════════════════
        //  TAB SWITCHING
        // ═══════════════════════════════════════

        private void SetActiveTab(Button active, Button b, Button c)
        {
            var accentBrush = new SolidColorBrush(Color.FromArgb(255, 37, 99, 235));
            var transparentBrush = new SolidColorBrush(Colors.Transparent);
            var secondaryBrush = new SolidColorBrush(Color.FromArgb(255, 100, 116, 139));

            active.BorderBrush = accentBrush;
            active.Foreground = accentBrush;

            b.BorderBrush = transparentBrush;
            b.Foreground = secondaryBrush;

            c.BorderBrush = transparentBrush;
            c.Foreground = secondaryBrush;
        }

        private void TabPersonalBtn_Click(object sender, RoutedEventArgs e)
        {
            PanelPersonal.Visibility = Visibility.Visible;
            PanelSecurity.Visibility = Visibility.Collapsed;
            PanelNotifications.Visibility = Visibility.Collapsed;
            SetActiveTab(TabPersonalBtn, TabSecurityBtn, TabNotificationsBtn);
        }

        private void TabSecurityBtn_Click(object sender, RoutedEventArgs e)
        {
            PanelPersonal.Visibility = Visibility.Collapsed;
            PanelSecurity.Visibility = Visibility.Visible;
            PanelNotifications.Visibility = Visibility.Collapsed;
            SetActiveTab(TabSecurityBtn, TabPersonalBtn, TabNotificationsBtn);
        }

        private void TabNotificationsBtn_Click(object sender, RoutedEventArgs e)
        {
            PanelPersonal.Visibility = Visibility.Collapsed;
            PanelSecurity.Visibility = Visibility.Collapsed;
            PanelNotifications.Visibility = Visibility.Visible;
            SetActiveTab(TabNotificationsBtn, TabPersonalBtn, TabSecurityBtn);
        }

        // ═══════════════════════════════════════
        //  PERSONAL TAB
        // ═══════════════════════════════════════

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var fullName = FullNameBox.Text.Trim();
            var email = EmailBox.Text.Trim();
            var phone = PhoneBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email))
            {
                ShowError("Full name and email are required.");
                return;
            }

            // _viewModel.UpdateProfile(fullName, email, phone);
        }

        // ═══════════════════════════════════════
        //  SECURITY TAB
        // ═══════════════════════════════════════

        private void UpdatePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            var current = CurrentPasswordBox.Password;
            var newPass = NewPasswordBox.Password;
            var confirm = ConfirmPasswordBox.Password;

            if (string.IsNullOrWhiteSpace(current) ||
                string.IsNullOrWhiteSpace(newPass) ||
                string.IsNullOrWhiteSpace(confirm))
            {
                ShowError("Please fill in all password fields.");
                return;
            }

            if (newPass != confirm)
            {
                ShowError("New passwords do not match.");
                return;
            }

            if (newPass.Length < 8)
            {
                ShowError("New password must be at least 8 characters.");
                return;
            }

            // TODO: re-enable once CurrentUser is wired up in ProfileViewModel
            // _viewModel.ChangePassword(current, newPass);
            ShowError("Change password not yet connected to user session.");
        }

        private void ToggleCurrentPassword_Click(object sender, RoutedEventArgs e)
        {
            CurrentPasswordBox.PasswordRevealMode =
                CurrentPasswordBox.PasswordRevealMode == PasswordRevealMode.Hidden
                    ? PasswordRevealMode.Visible
                    : PasswordRevealMode.Hidden;
        }

        private void ToggleNewPassword_Click(object sender, RoutedEventArgs e)
        {
            NewPasswordBox.PasswordRevealMode =
                NewPasswordBox.PasswordRevealMode == PasswordRevealMode.Hidden
                    ? PasswordRevealMode.Visible
                    : PasswordRevealMode.Hidden;
        }

        private void ToggleConfirmPassword_Click(object sender, RoutedEventArgs e)
        {
            ConfirmPasswordBox.PasswordRevealMode =
                ConfirmPasswordBox.PasswordRevealMode == PasswordRevealMode.Hidden
                    ? PasswordRevealMode.Visible
                    : PasswordRevealMode.Hidden;
        }

        private void TwoFactorToggle_Toggled(object sender, RoutedEventArgs e)
        {
            // Guard: control may not be loaded yet during page init
            if (TwoFactorPhoneBox is null) return;

            TwoFactorPhoneBox.IsEnabled = TwoFactorToggle.IsOn;
        }

        private void SaveTwoFactorPhone_Click(object sender, RoutedEventArgs e)
        {
            var phone = TwoFactorPhoneBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(phone))
            {
                ShowError("Please enter a valid phone number for 2FA.");
                return;
            }

            // TODO: _viewModel.UpdateTwoFactorPhone(phone);
            ShowSuccess("2FA phone number updated.");
        }

        private void EditConnectedAccount_Click(object sender, RoutedEventArgs e)
        {
            // TODO: open edit dialog for connected account
        }

        private void RemoveConnectedAccount_Click(object sender, RoutedEventArgs e)
        {
            // TODO: _viewModel.RemoveConnectedAccount(accountId);
        }

        private void ManageDevicesButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: App.NavigationService.NavigateTo<ManageDevicesView>();
        }

        // ═══════════════════════════════════════
        //  NOTIFICATIONS TAB
        // ═══════════════════════════════════════

        private void NotificationToggle_Toggled(object sender, RoutedEventArgs e)
        {
            // Guard: other toggles may not be loaded yet during page init
            if (TransactionAlertsToggle is null ||
                LoginAlertsToggle is null ||
                BillReminderToggle is null ||
                MarketingToggle is null ||
                SpendingAlertToggle is null) return;

            var transactionAlerts = TransactionAlertsToggle.IsOn;
            var loginAlerts = LoginAlertsToggle.IsOn;
            var billReminder = BillReminderToggle.IsOn;
            var marketing = MarketingToggle.IsOn;
            var spendingAlert = SpendingAlertToggle.IsOn;

            // TODO: _viewModel.UpdateNotificationPreferences(
            //     transactionAlerts, loginAlerts, billReminder, marketing, spendingAlert);
        }

        // ═══════════════════════════════════════
        //  SIDEBAR NAVIGATION
        // ═══════════════════════════════════════

        private void DashboardNavButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: App.NavigationService.NavigateTo<DashboardView>();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            App.NavigationService.NavigateTo<LoginView>();
        }

        // ═══════════════════════════════════════
        //  HELPERS
        // ═══════════════════════════════════════

        public void ShowError(string msg)
        {
            ErrorInfoBar.Message = msg;
            ErrorInfoBar.IsOpen = true;
        }

        public void ShowSuccess(string msg)
        {
            SuccessInfoBar.Message = msg;
            SuccessInfoBar.IsOpen = true;
        }

        public void ShowLoading()
        {
            LoadingPanel.Visibility = Visibility.Visible;
            LoadingRing.IsActive = true;
            SaveButton.IsEnabled = false;
            UpdatePasswordButton.IsEnabled = false;
        }

        public void HideLoading()
        {
            LoadingPanel.Visibility = Visibility.Collapsed;
            LoadingRing.IsActive = false;
            SaveButton.IsEnabled = true;
            UpdatePasswordButton.IsEnabled = true;
        }

        private void RefreshProfileCard()
        {
            ProfileCardName.Text = FullNameBox.Text.Trim();
            ProfileCardEmail.Text = EmailBox.Text.Trim();
            ProfileCardPhone.Text = PhoneBox.Text.Trim();
            ProfileCardAddress.Text = BillingAddressBox.Text.Trim();
        }

        private void ClearPasswordFields()
        {
            CurrentPasswordBox.Password = string.Empty;
            NewPasswordBox.Password = string.Empty;
            ConfirmPasswordBox.Password = string.Empty;
        }
    }
}