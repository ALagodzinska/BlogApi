using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<BlogPostController> _logger;
        private readonly BloggingContext _context;

        public UserController(ILogger<BlogPostController> logger, BloggingContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public ActionResult<string> GetUser() {
            if (User.Identity == null) throw new Exception("No user found!");
            return User.Identity.Name;
        }
    }
}
