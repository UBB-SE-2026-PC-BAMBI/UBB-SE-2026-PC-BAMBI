using BankApp.Server.DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace BankApp.Server.Controllers
{
    // This controller is just used for testing endpoints, have fun :) <3 B

    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet("db")]
        public IActionResult TestDb([FromServices] AppDbContext db)
        {
            try
            {
                var reader = db.ExecuteQuery("SELECT COUNT(*) FROM [User]", Array.Empty<object>());
                reader.Read();
                var count = reader.GetInt32(0);
                reader.Close();
                return Ok(new { message = "Connection works!", userCount = count });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, inner = ex.InnerException?.Message });
            }
        }
    }
}
