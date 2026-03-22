using BankApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankApp.Models.DTOs.Profile
{
    public class GetProfileResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int? UserId { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Nationality { get; set; }

        public GetProfileResponse(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public GetProfileResponse(bool success, string message, User user)
        {
            Success = success;
            Message = message;

            if (user != null)
            {
                UserId = user.Id;
                Email = user.Email;
                FullName = user.FullName;
                PhoneNumber = user.PhoneNumber;
                Address = user.Address;
                Nationality = user.Nationality;
            }
        }
    }
}
