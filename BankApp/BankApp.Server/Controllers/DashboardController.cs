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
            throw new NotImplementedException();
        }
    }
}