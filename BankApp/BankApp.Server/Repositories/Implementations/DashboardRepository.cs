using BankApp.Models.Entities;
using BankApp.Server.Repositories.Interfaces;
using BankApp.Server.DataAccess.Interfaces;

namespace BankApp.Server.Repositories.Implementations
{
	public class DashboardRepository : IDashboardRepository
	{
		private readonly IAccountDAO accountDAO;
		private readonly ICardDAO cardDAO;
		private readonly ITransactionDAO transactionDAO;
		private readonly INotificationDAO notificationDAO;

		public DashboardRepository(IAccountDAO accountDAO, ICardDAO cardDAO, ITransactionDAO transactionDAO, INotificationDAO notificationDAO)
		{
			this.accountDAO = accountDAO;
			this.cardDAO = cardDAO;
			this.transactionDAO = transactionDAO;
			this.notificationDAO = notificationDAO;
		}

		public List<Account> GetAccountsByUser(int userId)
		{
			return this.accountDAO.FindByUserId(userId);
		}
		public List<Card> GetCardsByUser(int userId)
		{
			return this.cardDAO.FindByUserId(userId);
		}
		public List<Transaction> GetRecentTransactions(int userId, int limit = 10)
		{
			return this.transactionDAO.FindRecentByAccountId(userId, limit);
		}
		public int GetUnreadNotificationCount(int userId)
		{
			return this.notificationDAO.CountUnreadByUserId(userId);
		}
	}
}