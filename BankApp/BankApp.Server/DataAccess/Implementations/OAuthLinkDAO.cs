using BankApp.Models.Entities;
using BankApp.Server.DataAccess.Interfaces;

namespace BankApp.Server.DataAccess.Implementations
{
    public class OAuthLinkDAO : IOAuthLinkDAO
    {
        public bool Create(int userId, string provider, string providerUserId, string? providerEmail)
        {
            // TODO: Marius
            return false;
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            // TODO: Marius
            throw new NotImplementedException();
        }

        public OAuthLink? FindByProvider(string provider, string providerUserId)
        {
            // TODO: Marius
            return null;
            throw new NotImplementedException();
        }

        public List<OAuthLink> FindByUserId(int userId)
        {
            //TODO: Marius
            return new List<OAuthLink>();
            throw new NotImplementedException();
        }
    }
}
