using BankApp.Client.Utilities;
using BankApp.Client.ViewModels.Base;
using BankApp.Models.Enums;

namespace BankApp.Client.ViewModels
{
    public class TwoFactorViewModel : BaseViewModel
    {
        public Observable<TwoFactorState> State { get; private set; }

        public void VerifyOTP(string otp)
        {
            throw new System.NotImplementedException();
        }

        public void ResendOTP()
        {
            throw new System.NotImplementedException();
        }

        public bool OpenLogin()
        {
            throw new System.NotImplementedException();
        }

        public bool OpenDashboard()
        {
            throw new System.NotImplementedException();
        }

        public override void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}