using BankApp.Server.Services.Infrastructure.Interfaces;

namespace BankApp.Server.Services.Infrastructure.Implementations
{
    public class EmailService : IEmailService
    {
        public void sendLockNotification(string email)
        {
            //throw new NotImplementedException();
        }

        public void SendLoginAlert(string email)
        {
            //throw new NotImplementedException();
        }

        public void sendOTPCode(string email, string code)
        {
            //throw new NotImplementedException();
        }

        public void sendPasswordResetLink(string email, string token)
        {
            //throw new NotImplementedException();
        }
    }
}
