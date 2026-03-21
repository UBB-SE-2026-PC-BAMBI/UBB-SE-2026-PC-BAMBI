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
        public User CurrentUser { get; private set; }
        public List<Card> Cards { get; private set; }
        public List<Transaction> RecentTransactions { get; private set; }
        public int UnreadNotificationCount { get; private set; }

        private readonly ApiService _apiService;

        public DashboardViewModel(ApiService apiService)
        {
            _apiService = apiService;
            State = new Observable<DashboardState>(DashboardState.Loading);
            Cards = new List<Card>();
            RecentTransactions = new List<Transaction>();
            UnreadNotificationCount = 0;
        }

        public async void LoadDashboard()
        {
            throw new NotImplementedException();
        }

        public async void LoadCards() 
        {
            throw new NotImplementedException(); 
        }

        public async void LoadRecentTransactions()
        {
            throw new NotImplementedException();
        }

        public async void LoadUnreadNotificationCount()
        {
            throw new NotImplementedException();
        }
        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}