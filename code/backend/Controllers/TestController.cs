using Microsoft.AspNetCore.Authorization;

namespace backend.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TestController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("test")]
        public async Task<IActionResult> TestAsync()
        {
            var existingUser = await _userManager.FindByEmailAsync("string@gmail.com");
            if (existingUser == null)
            {
                return NotFound("User not found.");
            }
            await _context.AddAsync(new Deck
            {
                Name = "Test Deck",
                UserId = existingUser.Id
            });
            await _context.SaveChangesAsync();
            return Ok("Deck created successfully.");
        }
    }
}
