using BankApp.Client.Utilities;
using BankApp.Client.ViewModels.Base;
using BankApp.Models.Enums;
using System;
namespace BankApp.Client.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        public Observable<RegisterState> State { get; private set; }

        public RegisterViewModel()
        {
            State = new Observable<RegisterState>(RegisterState.Idle);
            // maybe more!!
        }

        public void Register(string email, string password, string confirmPassword)
        {
            throw new NotImplementedException();
        }

        public void OAuthRegister(string email, string provider)
        {
            throw new NotImplementedException();
        }

        public bool OpenLogin()
        {
            throw new NotImplementedException();
        }

        public bool OpenForgotPassword()
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}