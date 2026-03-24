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

        // GET: api/profile/{userId}
        // Serializes: GetProfileResponse
        [HttpGet("{userId}")]
        public IActionResult GetProfile(int userId)
        {
            User? user = _profileService.GetUserById(userId);
            if (user == null)
            {
                return NotFound(new GetProfileResponse(false, "User not found."));
            }

            return Ok(new GetProfileResponse(true, "Successfully retrieved profile information.", user));
        }

        // PUT: api/profile/{userId}
        // Serializes: UpdateProfileResponse
        [HttpPut("{userId}")]
        public IActionResult UpdateProfile(int userId, [FromBody] UpdateProfileRequest request)
        {
            if (userId != request.UserId)
            {
                return BadRequest(new UpdateProfileResponse(false, "URL user id and current user mismatch."));
            }

            UpdateProfileResponse response = _profileService.UpdatePersonalInfo(request);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        // PUT: api/profile/{userId}/password
        // Serializes: ChangePasswordResponse
        [HttpPut("{userId}/password")]
        public IActionResult ChangePassword(int userId, [FromBody] ChangePasswordRequest request)
        {
            if (userId != request.UserId)
            {
                return BadRequest(new UpdateProfileResponse(false, "URL user id and current user mismatch."));
            }

            ChangePasswordResponse response = _profileService.ChangePassword(request);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        // GET: api/profile/{userId}/oauthlinks
        // Serializes: List<OAuthLink>
        [HttpGet("{userId}/oauthlinks")]
        public IActionResult GetOAuthLinks(int userId)
        {
            List<OAuthLink> links = _profileService.GetOAuthLinks(userId);

            if (links.Count == 0)
            {
                return NotFound(links);
            }

            return Ok(links);
        }

        // GET: api/profile/{userId}/notifications/preferences
        // Serializes: List<NotificationPreference>
        [HttpGet("{userId}/notifications/preferences")]
        public IActionResult GetNotificationPreferences(int userId)
        {
            List<NotificationPreference> prefs = _profileService.GetNotificationPreferences(userId);

            if (prefs.Count == 0)
            {
                return NotFound(prefs);
            }

            return Ok(prefs);
        }

        // PUT: api/profile/{userId}/notifications/preferences
        // Serializes: -
        [HttpPut("{userId}/notifications/preferences")]
        public IActionResult UpdateNotificationPreferences(int userId, [FromBody] List<NotificationPreference> prefs)
        {
            bool success = _profileService.UpdateNotificationPreferences(userId, prefs);
            if (!success)
            {
                return BadRequest();
            }

            return Ok();
        }

        // GET: api/profile/{userId}/sessions
        [HttpGet("{userId}/sessions")]
        public IActionResult GetActiveSessions(int userId)
        {
            List<Session> sessions = _profileService.GetActiveSessions(userId);
            return Ok(sessions);
        }

        // DELETE: api/profile/{userId}/sessions/{sessionId}
        [HttpDelete("{userId}/sessions/{sessionId}")]
        public IActionResult RevokeSession(int userId, int sessionId)
        {
            bool success = _profileService.RevokeSession(userId, sessionId);

            if (!success)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}