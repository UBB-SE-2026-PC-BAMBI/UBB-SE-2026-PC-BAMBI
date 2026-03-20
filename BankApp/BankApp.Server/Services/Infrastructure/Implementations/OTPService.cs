using BankApp.Server.Services.Infrastructure.Interfaces;

namespace BankApp.Server.Services.Infrastructure.Implementations
{
    public class OTPService : IOTPService
    {
        public string GenerateSMSOTP(int userId)
        {
            return "000000";
            //throw new NotImplementedException();
        }

        public string GenerateTOTP(int userId)
        {
            return "000000";
            //throw new NotImplementedException();
        }

        public void InvalidateOTP(int userId)
        {
            //throw new NotImplementedException();
        }

        public bool IsExpired(DateTime expiredAt)
        {
            return expiredAt < DateTime.UtcNow;
            //throw new NotImplementedException();
        }

        public bool VerifySMSOTP(int userId, string code)
        {
            return false;
            //throw new NotImplementedException();
        }

        public bool VerifyTOTP(int userId, string code)
        {
            return false;
            //throw new NotImplementedException();
        }
    }
}
