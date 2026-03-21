using System.Text.RegularExpressions;

namespace BankApp.Server.Utilities
{
    public static class ValidationUtil
    {
        // MARIUS PLS VERIFY THIS UWU
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) { return false; }
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }

        public static bool IsStrongPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) { return false; }
            return password.Length >= 8
                && password.Any(char.IsUpper)
                && password.Any(char.IsLower)
                && password.Any(char.IsDigit);
        }

        public static bool IsValidOTP(string otp)
        {
            return !string.IsNullOrWhiteSpace(otp) && otp.Length == 6 && otp.All(char.IsDigit);
        }

        public static bool PasswordsMatch(string a, string b)
        {
            return a == b;
        }

        public static bool IsValidPhoneNumber(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return false;
            return Regex.IsMatch(phone, @"^\+?[\d\s\-().]{7,15}$");
        }
    }
}