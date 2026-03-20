using Microsoft.UI.Xaml.Controls;

namespace BankApp.Client.Master
{
    public class NavigationService : INavigationService
    {
        private Frame? _frame;

        public void SetFrame(Frame frame)
        {
            _frame = frame;
        }

        public void NavigateTo<Page>()
        {
            _frame?.Navigate(typeof(Page));
        }

        public void GoBack()
        {
            if (CanGoBack())
                _frame?.GoBack();
        }

        public bool CanGoBack()
        {
            return _frame?.CanGoBack ?? false;
        }
    }
}