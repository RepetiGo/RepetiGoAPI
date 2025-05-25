using Microsoft.AspNetCore.Authorization;

namespace backend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { message = "Test successful!" });
        }
    }
}
