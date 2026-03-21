using BankApp.Client.Utilities;
using BankApp.Client.ViewModels;
using BankApp.Models.Enums;
using BankApp.Models.Entities;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System;
using Windows.UI;

namespace BankApp.Client.Views
{
    public sealed partial class DashboardView : Page, Observer<DashboardState>
    {
        private readonly DashboardViewModel _viewModel;
        private int _currentCardIndex = 0;

        public DashboardView()
        {
            this.InitializeComponent();

            _viewModel = new DashboardViewModel(App.ApiService);
            _viewModel.State.AddObserver(this);
            _viewModel.LoadDashboard();
        }

        public void Update(DashboardState state)
        {
            OnStateChanged(state);
        }

        public void OnStateChanged(DashboardState state)
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                switch (state)
                {
                    case DashboardState.Loading:
                        // TODO: show loading indicator if needed
                        break;

                    case DashboardState.Success:
                        RefreshUI();
                        break;

                    case DashboardState.Error:
                        ShowError("Failed to load dashboard. Please try again.");
                        break;
                }
            });
        }

        private void RefreshUI()
        {
            TransactionsList.ItemsSource = _viewModel.RecentTransactions;
            BuildCardDots();
            ShowCard(_currentCardIndex);
        }

        // ─── CARD NAVIGATION ──────────────────────────────────────────

        private void ShowCard(int index)
        {
            var cards = _viewModel.Cards;
            if (cards == null || cards.Count == 0) return;

            index = Math.Clamp(index, 0, cards.Count - 1);
            _currentCardIndex = index;

            var card = cards[index];

            CardBankName.Text = "ING";
            CardBrandName.Text = string.IsNullOrEmpty(card.CardBrand) ? "Mastercard" : card.CardBrand;
            CardHolderText.Text = card.CardholderName.ToUpper();
            CardExpiryText.Text = card.ExpiryDate.ToString("MM/yy");

            var number = card.CardNumber;
            CardNumberText.Text = number.Length >= 4
                ? $"**** **** **** {number[^4..]}"
                : "**** **** **** ****";

            UpdateCardDots();
        }

        private void BuildCardDots()
        {
            CardDots.Children.Clear();
            var count = _viewModel.Cards?.Count ?? 0;
            for (int i = 0; i < count; i++)
            {
                var dot = new Ellipse
                {
                    Width = i == _currentCardIndex ? 18 : 8,
                    Height = 8,
                    RadiusX = 4,
                    RadiusY = 4,
                    Fill = new SolidColorBrush(i == _currentCardIndex
                        ? Color.FromArgb(255, 78, 205, 196)
                        : Color.FromArgb(100, 78, 205, 196))
                };
                CardDots.Children.Add(dot);
            }
        }

        private void UpdateCardDots()
        {
            for (int i = 0; i < CardDots.Children.Count; i++)
            {
                if (CardDots.Children[i] is Ellipse dot)
                {
                    dot.Width = i == _currentCardIndex ? 18 : 8;
                    dot.Fill = new SolidColorBrush(i == _currentCardIndex
                        ? Color.FromArgb(255, 78, 205, 196)
                        : Color.FromArgb(100, 78, 205, 196));
                }
            }
        }

        private void PrevCardButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentCardIndex > 0)
                ShowCard(_currentCardIndex - 1);
        }

        private void NextCardButton_Click(object sender, RoutedEventArgs e)
        {
            var count = _viewModel.Cards?.Count ?? 0;
            if (_currentCardIndex < count - 1)
                ShowCard(_currentCardIndex + 1);
        }

        // ─── QUICK ACTIONS ────────────────────────────────────────────

        private async void TransferButton_Click(object sender, RoutedEventArgs e)
        {
            await ShowAlertAsync("Transfer", "Transfer feature works! ✅");
        }

        private async void PayBillButton_Click(object sender, RoutedEventArgs e)
        {
            await ShowAlertAsync("Pay Bill", "Pay Bill feature works! ✅");
        }

        private async void ExchangeButton_Click(object sender, RoutedEventArgs e)
        {
            await ShowAlertAsync("Currency Exchange", "Currency Exchange feature works! ✅");
        }

        private async void TxHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            await ShowAlertAsync("Transaction History", "Transaction History feature works! ✅");
        }

        // ─── NAV + LOGOUT ─────────────────────────────────────────────

        private void NavItem_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            // TODO: navigate to respective views when implemented
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            App.ApiService.ClearToken();
            App.NavigationService.NavigateTo<LoginView>();
        }

        // ─── HELPER ──────────────────────────────────────────────────

        private void ShowError(string msg)
        {
            // TODO: add an ErrorInfoBar to the XAML like LoginView has
        }

        private async System.Threading.Tasks.Task ShowAlertAsync(string title, string message)
        {
            var dialog = new ContentDialog
            {
                Title = title,
                Content = message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            await dialog.ShowAsync();
        }
    }
}
