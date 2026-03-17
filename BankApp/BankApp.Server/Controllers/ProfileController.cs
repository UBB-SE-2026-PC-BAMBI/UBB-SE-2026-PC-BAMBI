using Microsoft.AspNetCore.Mvc;
using BankApp.Server.Services.Interfaces;
using BankApp.Models.DTOs.Profile;
using BankApp.Models.Entities;

namespace BankApp.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;
        public ProfileController(IProfileService profileService) { _profileService = profileService; }

        [HttpGet("{userId}")]
        public IActionResult GetProfile(int userId)
        {
            throw new NotImplementedException();
        }

        [HttpPut("{userId}")]
        public IActionResult UpdateProfile(int userId, [FromBody] UpdateProfileRequest request)
        {
            throw new NotImplementedException();
        }

        [HttpPut("{userId}/password")]
        public IActionResult ChangePassword(int userId, [FromBody] ChangePasswordRequest request)
        {
            throw new NotImplementedException();
        }

        [HttpGet("{userId}/notifications/preferences")]
        public IActionResult GetNotificationPreferences(int userId)
        {
            throw new NotImplementedException();
        }

        [HttpPut("{userId}/notifications/preferences")]
        public IActionResult UpdateNotificationPreferences(int userId, [FromBody] List<NotificationPreference> prefs)
        {
            throw new NotImplementedException();
        }
    }
}