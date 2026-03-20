using Microsoft.AspNetCore.Mvc;
using BankApp.Server.Services.Interfaces;

namespace BankApp.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashService;
        public DashboardController(IDashboardService dashService) { _dashService = dashService; }

        [HttpGet("{userId}")]
        public IActionResult GetDashboard(int userId)
        {
            try
            {
                var dashboardData = _dashService.GetDashboardData(userId);
                if (dashboardData == null)
                {
                    return NotFound(new { message = $"User with Id = {userId} was not found." });

                }

                return Ok(dashboardData);

            }
            catch (Exception ex)
            {
                return StatusCode(500,
                    new { error = "An error occured while fetching the dashboard data."});
            }
        }
    }
}