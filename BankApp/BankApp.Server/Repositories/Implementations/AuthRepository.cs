using BankApp.Server.Repositories.Interfaces;
using BankApp.Server.DataAccess.Interfaces;
using BankApp.Models.Entities;

namespace BankApp.Server.Repositories.Implementations
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IUserDAO _userDao;
        private readonly ISessionDAO _sessionDao;
        private readonly IOAuthLinkDAO _oAuthLinkDao;
        private readonly IPasswordResetTokenDAO _passwordResetTokenDao;

        public AuthRepository(IUserDAO userDao, ISessionDAO sessionDao, IOAuthLinkDAO oAuthLinkDao, IPasswordResetTokenDAO passwordResetTokenDao)
        {
            _userDao = userDao;
            _sessionDao = sessionDao;
            _oAuthLinkDao = oAuthLinkDao;
            _passwordResetTokenDao = passwordResetTokenDao;
        }

        // USER 

        public User? FindUserByEmail(string email)
        {
            return _userDao.FindByEmail(email);
        }

        public bool CreateUser(User user)
        {
            return _userDao.Create(user);
        }

        // OAUTH

        public OAuthLink? FindOAuthLink(string provider, string providerUserId)
        {
            return _oAuthLinkDao.FindByProvider(provider, providerUserId);
        }

        public bool CreateOAuthLink(OAuthLink link)
        {
            return _oAuthLinkDao.Create(link.UserId, link.Provider, link.ProviderUserId, link.ProviderEmail);
        }

        // SESSIONS

        public Session CreateSession(int userId, string token, string? deviceInfo, string? browser, string? ip)
        {
            return _sessionDao.Create(userId, token, deviceInfo, browser, ip);
        }

        public Session? FindSessionByToken(string token)
        {
            return _sessionDao.FindByToken(token);
        }

        public void InvalidateAllSessions(int userId)
        {
            _sessionDao.RevokeAll(userId);
        }

        public List<Session> FindSessionsByUserId(int userId)
        {
            return _sessionDao.FindByUserId(userId);
        }

        public bool UpdateSessionToken(int sessionId)
        {
            // Revoke the old session
            // the service layer will create a new one
            _sessionDao.Revoke(sessionId);
            return true;
        }

        public void SavePasswordResetToken(PasswordResetToken token)
        {
            _passwordResetTokenDao.Create(token.UserId, token.TokenHash, token.ExpiresAt);
        }

        public PasswordResetToken? FindPasswordResetToken(string tokenHash)
        {
            return _passwordResetTokenDao.FindByToken(tokenHash);
        }

        // ACCOUNT SECURITY
        public void IncrementFailedAttempts(int userId)
        {
            _userDao.IncrementFailedAttempts(userId);
        }

        public void ResetFailedAttempts(int userId)
        {
            _userDao.ResetFailedAttempts(userId);
        }

        public void LockAccount(int userId, DateTime lockoutEnd)
        {
            _userDao.LockAccount(userId, lockoutEnd);
        }
    }
}
