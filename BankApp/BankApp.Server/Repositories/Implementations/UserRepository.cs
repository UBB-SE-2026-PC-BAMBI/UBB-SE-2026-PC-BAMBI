using BankApp.Models.Entities;
using BankApp.Server.Repositories.Interfaces;

namespace BankApp.Server.Repositories
{
    public class UserRepository : IUserRepository
    {

        // OVERRIDE THIS DUMMY IMPLEMENTATION WITH THE REAL IMPLEMENTATION
        public User? FindById(int userId) =>
            throw new NotImplementedException();

        public bool UpdateUser(User user) =>
            throw new NotImplementedException();

        public List<Session> GetActiveSessions(int userId) =>
            throw new NotImplementedException();

        public void RevokeSession(int sessionId) =>
            throw new NotImplementedException();

        public List<OAuthLink> GetLinkedProviders(int userId) =>
            throw new NotImplementedException();

        public bool SaveOAuthLink(int userId, string provider, string providerUserId, string? email) =>
            throw new NotImplementedException();

        public void DeleteOAuthLink(int linkId) =>
            throw new NotImplementedException();

        public List<NotificationPreference> GetNotificationPreferences(int userId) =>
            throw new NotImplementedException();

        public bool UpdateNotificationPreferences(int userId, List<NotificationPreference> prefs) =>
            throw new NotImplementedException();
    }
}