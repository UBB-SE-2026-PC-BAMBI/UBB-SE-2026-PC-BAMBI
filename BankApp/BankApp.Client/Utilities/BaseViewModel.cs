using System;

namespace BankApp.Client.Utilities
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public void OnPropertyChanged(string propertyName)
        {
            throw new NotImplementedException();
        }

        protected abstract void SetState<T>(Observable<T> observable, T value);
        public abstract void Dispose();
    }
}
