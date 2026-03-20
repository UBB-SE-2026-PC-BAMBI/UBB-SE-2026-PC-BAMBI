using BankApp.Models.Entities;
using BankApp.Server.DataAccess.Interfaces;

namespace BankApp.Server.DataAccess.Implementations
{
    public class PasswordResetTokenDAO : IPasswordResetTokenDAO
    {
        public PasswordResetToken Create(int userId, string tokenHash, DateTime expiresAt)
        {
            //TODO: Marius
            return null;
            //throw new NotImplementedException();
        }

        public void DeleteExpired()
        {
            //throw new NotImplementedException();
        }

        public PasswordResetToken? FindByToken(string tokenHash)
        {
            //TODO: Marius
            return null;
            throw new NotImplementedException();
        }

        public void MarkAsUsed(int tokenId)
        {
            //throw new NotImplementedException();
        }
    }
}
