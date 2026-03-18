using System.Security.Claims;

namespace BankApp.Server.Services.Infrastructure;

public interface IJWTService
{
    string GenerateToken(int userId);
    ClaimsPrincipal? ValidateToken(string token);
    int? ExtractUserId(string token);
}