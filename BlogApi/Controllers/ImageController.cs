using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly ILogger<BlogPostController> _logger;
        private readonly BloggingContext _context;

        public ImageController(ILogger<BlogPostController> logger, BloggingContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public FileResult PreviewImage(int postId)
        {
            var post = _context.Posts.Where(post => post.BlogPostId == postId).FirstOrDefault();

            if (post != null)
            {
                return File(post.PreviewImage, "image/png");
            }
            else
            {
                throw new Exception(String.Format("No blog post found with id {0}", postId));
            }
        }

        [HttpGet]
        public FileResult BackgroundImage(int postId)
        {
            var post = _context.Posts.Where(post => post.BlogPostId == postId).FirstOrDefault();

            if (post != null)
            {
                return File(post.BackgroundImage, "image/png");
            }
            else
            {
                throw new Exception(String.Format("No blog post found with id {0}", postId));
            }
        }
    }
}
