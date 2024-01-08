using BlogApi;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BlogPostController
    {
        private readonly ILogger<BlogPostController> _logger;
        private readonly BloggingContext _context;

        int resultsPerPage = 5;

        public BlogPostController(ILogger<BlogPostController> logger, BloggingContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost]
        public ActionResult<int> PostBlog(BlogPost post) 
        {
            post.CreationDate = DateTime.Now;
            _context.Posts.Add(post);
            _context.SaveChanges();
            
            return post.BlogPostId;
        }

        [HttpGet]        
        public double GetPageCount()
        {
            return Math.Ceiling(_context.Posts.Count() / (double)resultsPerPage);
        }

        [HttpGet]
        public List<BlogPost> GetPostsForPage(int page)
        {
            int position = 5 * (page - 1);

            var nextPage = _context.Posts
                .OrderByDescending(b => b.CreationDate)
                .Skip(position)
                .Take(resultsPerPage)
                .ToList();
            return nextPage;
        }

        [HttpGet]
        public BlogPost? GetPost(int postId)
        {
            return _context.Posts.Where(post => post.BlogPostId == postId).FirstOrDefault();
        }
    }
}