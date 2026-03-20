using Microsoft.AspNetCore.Mvc;
using BankApp.Server.Services.Interfaces;
using BankApp.Models.DTOs.Auth;
using BankApp.Server.DataAccess;
using Microsoft.AspNetCore.Connections.Features;

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
            LoginResponse response = _authService.Login(request);
            
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            RegisterResponse response = _authService.Register(request);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
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

        [HttpPost("logout")]
        public IActionResult Logout([FromHeader(Name = "Authorization")] string authorization)
        {
            // Bogdan: this implementation is not enough, still need to invalidate JWT, but this is not on original diagram
            // can expand in the future.
            if (string.IsNullOrWhiteSpace(authorization) || !authorization.StartsWith("Bearer "))
            {
                return BadRequest(new { error = "No token provided." });
            }

            string token = authorization.Substring("Bearer ".Length);

            if (!_authService.Logout(token))
            {
                return BadRequest(new { error = "Invalid session." });
            }

            return Ok(new { message = "Logged out successfully." });
        }
    }
}