using BankApp.Models.DTOs.Auth;
using BankApp.Models.Entities;
using BankApp.Server.Repositories.Interfaces;
using BankApp.Server.Services.Infrastructure.Implementations;
using BankApp.Server.Services.Infrastructure.Interfaces;
using BankApp.Server.Services.Interfaces;
using BankApp.Server.Utilities;

namespace BankApp.Server.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IHashService _hashService;
        private readonly IJWTService _jwtService;
        private readonly IOTPService _otpService;
        private readonly IEmailService _emailService;

        private const int MaxFailedAttempts = 5;
        private const int LockoutMinutes = 30;

        public AuthService(IAuthRepository authRepository, IHashService hashService, IJWTService jwtService, IOTPService otpService, IEmailService emailService)
        {
            _authRepository = authRepository;
            _hashService = hashService;
            _jwtService = jwtService;
            _otpService = otpService;
            _emailService = emailService;
        }

        public LoginResponse Login(LoginRequest request)
        {
            if (!ValidationUtil.IsValidEmail(request.Email))
            {
                return new LoginResponse { Success = false, Error = "Invalid mail format." };
            }

            User? user = _authRepository.FindUserByEmail(request.Email);
            if (user == null)
            {
                return new LoginResponse { Success = false, Error = "Invalid email or password." };
            }

            LoginResponse? lockCheck = CheckAccountLock(user);
            if (lockCheck != null)
            {
                return lockCheck;
            }

            if (!_hashService.Verify(request.Password, user.PasswordHash))
            {
                return HandleFailedPassword(user);
            }

            if (user.Is2FAEnabled)
            {
                return Handle2FA(user);
            }

            return CompleteLogin(user);
        }

        public RegisterResponse Register(RegisterRequest request)
        {
            string? validationError = ValidateRegistration(request);
            if (validationError != null)
            {
                return new RegisterResponse { Success = false, Error = validationError };
            }

            User? existingUser = _authRepository.FindUserByEmail(request.Email);
            if (existingUser != null)
            {
                return new RegisterResponse { Success = false, Error = "Email is already registered." };
            }

            User user = CreateUserFromRequest(request);
            bool created = _authRepository.CreateUser(user);

            if (!created)
            {
                return new RegisterResponse { Success = false, Error = "Failed to create account." };
            }

            return new RegisterResponse { Success = true };
        }

        public LoginResponse OAuthLogin(OAuthLoginRequest request)
        {
            // TODO: Marius, OAuthLinkDAO
            throw new NotImplementedException();
        }

        public RegisterResponse OAuthRegister(OAuthRegisterRequest request)
        {
            // TODO: Marius, OAuthLinkDAO
            throw new NotImplementedException();
        }

        public LoginResponse VerifyOTP(VerifyOTPRequest request)
        {
            // TODO: Marius
            throw new NotImplementedException();
        }

        public void ResendOTP(int userId, string method)
        {
            // TODO: Marius
            throw new NotImplementedException();
        }

        public void RequestPasswordReset(string email)
        {
            // TODO: Marius
            throw new NotImplementedException();
        }

        public bool ResetPassword(string token, string newPasswordHash)
        {
            // TODO: Marius
            throw new NotImplementedException();
        }

        // PRIVATE HELPERS
        private LoginResponse? CheckAccountLock(User user)
        {
            if (!user.IsLocked)
            {
                return null;
            }
                
            if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
            {
                return new LoginResponse { Success = false, Error = "Account is locked. Try again later." };
            }

            // Lockout expired, reset and allow login attempt
            _authRepository.ResetFailedAttempts(user.Id);
            return null;
        }

        private LoginResponse HandleFailedPassword(User user)
        {
            _authRepository.IncrementFailedAttempts(user.Id);

            if (user.FailedLoginAttempts + 1 >= MaxFailedAttempts)
            {
                _authRepository.LockAccount(user.Id, DateTime.UtcNow.AddMinutes(LockoutMinutes));
                _emailService.sendLockNotification(user.Email);
                return new LoginResponse { Success = false, Error = "Account locked due to too many failed attempts." };
            }

            return new LoginResponse { Success = false, Error = "Invalid email or password." };
        }

        private LoginResponse Handle2FA(User user)
        {
            string otp = _otpService.GenerateTOTP(user.Id);

            if (user.Preferred2FAMethod == "email")
            {
                _emailService.sendOTPCode(user.Email, otp);
            }

            return new LoginResponse
            {
                Success = true,
                Requires2FA = true,
                UserId = user.Id,
                Token = null
            };
        }

        private LoginResponse CompleteLogin(User user)
        {
            _authRepository.ResetFailedAttempts(user.Id);
            string token = _jwtService.GenerateToken(user.Id);
            _authRepository.CreateSession(user.Id, token, null, null, null);
            _emailService.SendLoginAlert(user.Email);
            return new LoginResponse
            {
                Success = true,
                Token = token,
                Requires2FA = false,
                UserId = user.Id
            };
        }

        private string? ValidateRegistration(RegisterRequest request)
        {
            // There should also be client-side validation, this is last resort
            // can't trust the client

            if (!ValidationUtil.IsValidEmail(request.Email))
                return "Invalid email format.";

            if (!ValidationUtil.IsStrongPassword(request.Password))
                return "Password must be at least 8 characters with uppercase, lowercase, and a digit.";

            if (string.IsNullOrWhiteSpace(request.FullName))
                return "Full name is required.";

            return null;
        }

        private User CreateUserFromRequest(RegisterRequest request)
        {
            return new User
            {
                Email = request.Email,
                PasswordHash = _hashService.GetHash(request.Password),
                FullName = request.FullName,
                PreferredLanguage = "en",
                Is2FAEnabled = false,
                IsLocked = false,
                FailedLoginAttempts = 0
            };
        }
        public bool Logout(string token)
        {
            Session? session = _authRepository.FindSessionByToken(token);
            if (session == null)
            {
                return false;
            }
            _authRepository.UpdateSessionToken(session.Id);
            return true;
        }
    }
}
