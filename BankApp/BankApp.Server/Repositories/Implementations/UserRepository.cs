using BankApp.Models.Enums;

namespace BankApp.Server.Repositories.Implementations;

using BankApp.Models.Entities;
using BankApp.Server.DataAccess.Interfaces;
using BankApp.Server.Repositories.Interfaces;
public class UserRepository : IUserRepository
{
    private readonly IUserDAO _userDao;
    private readonly ISessionDAO _sessionDao;
    private readonly IOAuthLinkDAO _oAuthLinkDao;
    private readonly INotificationPreferenceDAO _notificationPreferenceDao;

    public UserRepository(IUserDAO userDao, ISessionDAO sessionDao, IOAuthLinkDAO oAuthLinkDao,
        INotificationPreferenceDAO notificationPreferenceDao)
    {
        _userDao = userDao;
        _sessionDao = sessionDao;
        _notificationPreferenceDao = notificationPreferenceDao;
        _oAuthLinkDao = oAuthLinkDao;
    }

    public User? FindById(int id)
    {
        return _userDao.FindById(id);
    }

    public bool UpdateUser(User user)
    {
        return _userDao.Update(user);
    }

    public bool UpdatePassword(int userId, string newPasswordHash)
    {
        return _userDao.UpdatePassword(userId, newPasswordHash);
    }

    public List<Session> GetActiveSessions(int userId)
    {
        return _sessionDao.FindByUserId(userId);
    }

    public void RevokeSession(int sessionId)
    {
        _sessionDao.Revoke(sessionId);
    }

    public List<OAuthLink> GetLinkedProviders(int userId)
    {
        return _oAuthLinkDao.FindByUserId(userId);
    }

    public bool SaveOAuthLink(int userId, string provider, string providerUserId, string? email)
    {
        return _oAuthLinkDao.Create(userId, provider, providerUserId, email);
    }

    public void DeleteOAuthLink(int linkId)
    {
        _oAuthLinkDao.Delete(linkId);
    }

    public List<NotificationPreference> GetNotificationPreferences(int userId)
    {
        //TODO: Change hardcoded examples until Register Model adds it 

        List<NotificationPreference> notificationPrefs = _notificationPreferenceDao.FindByUserId(userId);

        if (notificationPrefs.Count == 0)
        {
            return new List<NotificationPreference>
            {
                new NotificationPreference { UserId = userId, Category = NotificationType.Payment, PushEnabled = true, EmailEnabled = true, SmsEnabled = false, MinAmountThreshold = null },
                new NotificationPreference { UserId = userId, Category = NotificationType.InboundTransfer, PushEnabled = true, EmailEnabled = true, SmsEnabled = false, MinAmountThreshold = null },
                new NotificationPreference { UserId = userId, Category = NotificationType.OutboundTransfer, PushEnabled = true, EmailEnabled = true, SmsEnabled = false, MinAmountThreshold = null },
                new NotificationPreference { UserId = userId, Category = NotificationType.LowBalance, PushEnabled = true, EmailEnabled = true, SmsEnabled = true, MinAmountThreshold = null },
                new NotificationPreference { UserId = userId, Category = NotificationType.DuePayment, PushEnabled = true, EmailEnabled = true, SmsEnabled = true, MinAmountThreshold = null },
                new NotificationPreference { UserId = userId, Category = NotificationType.SuspiciousActivity, PushEnabled = true, EmailEnabled = true, SmsEnabled = true, MinAmountThreshold = null }
            };
        }

        return notificationPrefs;
        // DONT!!! REMOVE !!!
        //return _notificationPreferenceDao.FindByUserId(userId);
    }

    public bool UpdateNotificationPreferences(int userId, List<NotificationPreference> prefs)
    {
        return _notificationPreferenceDao.Update(userId, prefs);
    }
}