using BankApp.Client.Utilities;
using BankApp.Client.ViewModels.Base;
using BankApp.Models.Enums;
using BankApp.Models.DTOs.Auth;
using System;
using System.Threading.Tasks;
namespace BankApp.Client.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public Observable<LoginState> State { get; private set; }
        private readonly ApiService _apiService;

        public LoginViewModel(ApiService apiService)
        {
            State = new Observable<LoginState>(LoginState.Idle);
            _apiService = apiService;
        }

        public async void Login(string email, string password)
        {
            SetState(State, LoginState.Loading);

            try
            {
                LoginRequest request = new LoginRequest
                {
                    Email = email,
                    Password = password
                };

                LoginResponse? response = await _apiService.PostAsync<LoginRequest, LoginResponse>(
                    "/api/auth/login", request);

                if (response == null)
                {
                    SetState(State, LoginState.Error);
                    return;
                }

                if (!response.Success)
                {
                    HandleLoginError(response);
                    return;
                }

                if (response.Requires2FA)
                {
                    SetState(State, LoginState.Require2FA);
                    return;
                }

                // Login successful
                // Store the token and userId for future requests
                _apiService.SetToken(response.Token!);
                _apiService.SetCurrentUserId(response.UserId!.Value);
                SetState(State, LoginState.Success);
            }
            catch (Exception)
            {
                SetState(State, LoginState.Error);
            }
        }

        public async void OAuthLogin(string email, string provider)
        {
            throw new NotImplementedException();
        }

        private void HandleLoginError(LoginResponse response)
        {
            if (response.Error != null && response.Error.Contains("locked"))
            {
                SetState(State, LoginState.AccountLocked);
            }
            else
            {
                SetState(State, LoginState.InvalidCredentials);
            }
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}