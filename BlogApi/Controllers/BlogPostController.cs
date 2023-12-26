using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class BlogPostController
    {
        private readonly ILogger<BlogPostController> _logger;
        private readonly BloggingContext _context;

        public BlogPostController(ILogger<BlogPostController> logger, BloggingContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost]
        public ActionResult<int> PostBlog(BlogPost post) 
        {
            _context.Posts.Add(post);
            _context.SaveChanges();
            
            return post.BlogPostId;
        }
    }
}
