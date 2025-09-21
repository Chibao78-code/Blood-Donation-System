using Microsoft.AspNetCore.Mvc;

namespace BloodDonation.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { message = "Blood Donation API is running!", timestamp = DateTime.Now });
        }

        [HttpGet("health")]
        public IActionResult HealthCheck()
        {
            return Ok(new { status = "Healthy", service = "Blood Donation API" });
        }
    }
}