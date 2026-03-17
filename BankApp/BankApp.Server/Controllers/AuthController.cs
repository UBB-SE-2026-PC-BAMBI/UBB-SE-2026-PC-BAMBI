using Microsoft.AspNetCore.Mvc;
using BankApp.Server.Services.Interfaces;
using BankApp.Models.DTOs.Auth;

namespace BankApp.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService) { _authService = authService; }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            throw new NotImplementedException();
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            throw new NotImplementedException();
        }

        [HttpPost("verify-otp")]
        public IActionResult VerifyOTP([FromBody] VerifyOTPRequest request)
        {
            throw new NotImplementedException();
        }

        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            throw new NotImplementedException();
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest request)
        {
            throw new NotImplementedException();
        }
    }
}