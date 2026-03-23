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
            LoginResponse response = _authService.VerifyOTP(request);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new { error = "Email is required." });
            }
            _authService.RequestPasswordReset(request.Email);

            // Always return an OK response with a generic message ( prevent malicious operations )
            return Ok(new { message = "If an account with that email exists, a password reset link has been sent." });
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Token) || string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return BadRequest(new { error = "Token and new password are required." });
            }

            bool isSuccess = _authService.ResetPassword(request.Token, request.NewPassword);
            if (!isSuccess)
            {
                return BadRequest(new { error = "Invalid, expired, or already used reset token." });
            }

            return Ok(new { message = "Password reset successfully. You may now log in with your new password." });
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

        [HttpPost("resend-otp")]
        public IActionResult ResendOTP([FromQuery] int userId, [FromQuery] string method = "email")
        {
            _authService.ResendOTP(userId, method);
            return Ok(new { message = "If the user exists, a new code has been sent." });
        }
    }
}