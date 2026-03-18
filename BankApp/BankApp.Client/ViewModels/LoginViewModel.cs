using BankApp.Client.ViewModels.Base;
using BankApp.Models.Enums;
using BankApp.Client.Utilities;
using BankApp.Models.DTOs.Auth;
using System;
namespace BankApp.Client.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public Observable<LoginState> State { get; private set; }

        public LoginViewModel()
        {
            State = new Observable<LoginState>(LoginState.Idle);
            // maybe more
        }

        public void Login(string email, string password)
        {
            // Create a LoginRequest DTO and send it
            throw new NotImplementedException();
        }

        public void OAuthLogin(string email, string provider)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}