using System;

namespace BankApp.Client.Utilities
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public void OnPropertyChanged(string propertyName)
        {
            throw new NotImplementedException();
        }

        protected void SetState<T>(Observable<T> observable, T value)
        {
            throw new NotImplementedException();
        }
        public abstract void Dispose();
    }
}
