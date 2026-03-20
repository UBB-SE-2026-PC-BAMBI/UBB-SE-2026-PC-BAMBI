using BankApp.Models.Entities;
using BankApp.Server.Repositories.Interfaces;

namespace BankApp.Server.Repositories.Implementations
{
	public class DashboardRepository : IDashboardRepository
	{
		private readonly AccountDAO accountDAO;
		private readonly CardDAO cardDAO;
		private readonly TransactionDAO transactionDAO;
		private readonly NotificationDAO notificationDAO;

		public DashboardRepository(AccountDAO accountDAO, CardDAO cardDAO, TransactionDAO transactionDAO, NotificationDAO notificationDAO)
		{
			this.accountDAO = accountDAO;
			this.cardDAO = cardDAO;
			this.transactionDAO = transactionDAO;
			this.notificationDAO = notificationDAO;
		}

		public List<Account> GetAccountsByUser(int userId)
		{
			return this.accountDAO.GetAccountsByUser(userId);
		}
		public List<Card> GetCardsByUser(int userId)
		{
			return this.cardDAO.GetCardsByUser(userId);
		}
		public List<Transaction> GetRecentTransactions(int userId, int limit = 10)
		{
			return this.transactionDAO.GetRecentTransactions(userId, limit);
		}
		public int GetUnreadNotificationCount(int userId)
		{
			return this.notificationDAO.GetUnreadCountByUser(userId);
		}
	}
}