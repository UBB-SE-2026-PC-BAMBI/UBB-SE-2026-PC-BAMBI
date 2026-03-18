using BankApp.Client.Utilities;
using BankApp.Client.ViewModels.Base;
using BankApp.Models.Enums;
using System;
namespace BankApp.Client.ViewModels
{
    public class ForgotPasswordViewModel : BaseViewModel
    {
        public Observable<ForgotPasswordState> State { get; private set; }

        public void ForgotPassword(string email)
        {
            throw new NotImplementedException();
        }

        public void ResetPassword(string email, string newPassword, string code)
        {
            throw new NotImplementedException();
        }

        public bool OpenLogin()
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}