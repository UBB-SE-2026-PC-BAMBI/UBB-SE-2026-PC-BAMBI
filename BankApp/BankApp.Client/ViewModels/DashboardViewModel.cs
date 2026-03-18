using BankApp.Client.Utilities;
using BankApp.Client.ViewModels.Base;
using BankApp.Models.Entities;
using BankApp.Models.Enums;
using System.Collections.Generic;
using System;

namespace BankApp.Client.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        public Observable<DashboardState> State { get; private set; }
        public Observable<User> User { get; private set; }
        public Observable<List<Card>> Cards { get; private set; }
        public Observable<List<Transaction>> RecentTransactions { get; private set; }
        public Observable<int> UnreadNotificationCount { get; private set; }

        public void LoadDashboard()
        {
            throw new NotImplementedException();
        }

        public void LoadCards() 
        {
            throw new NotImplementedException(); 
        }

        public void LoadRecentTransactions()
        {
            throw new NotImplementedException();
        }

        public void LoadUnreadNotificationCount()
        {
            throw new NotImplementedException();
        }

        public bool OpenProfile()
        {
            throw new NotImplementedException();
        }

        public bool OpenTransfers()
        {
            throw new NotImplementedException();
        }
        public bool OpenPayBill()
        {
            throw new NotImplementedException();
        }
        public bool OpenExchange()
        {
            throw new NotImplementedException();
        }
        public bool OpenTransactionHistory()
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}