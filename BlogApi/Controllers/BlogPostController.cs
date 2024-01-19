using BlogApi.Models;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize]
        [HttpPost]
        public ActionResult<int> PostBlog(BlogPostInput postInput) 
        {
            var post = new BlogPost();
            post.Content = postInput.Content;
            post.Title = postInput.Title;
            post.PreviewImage = Convert.FromBase64String(postInput.PreviewImage);
            post.BackgroundImage = Convert.FromBase64String(postInput.BackgroundImage);
            post.CreationDate = DateTime.Now;
            post.User = "StacyGodz";

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
        public List<BlogPostInput> GetPostsForPage(int page)
        {
            int position = 5 * (page - 1);

            var nextPage = _context.Posts.Select(post => new BlogPostInput 
                                { BlogPostId = post.BlogPostId, CreationDate = post.CreationDate, 
                                Title = post.Title, Content = post.Content, User = post.User })
                .OrderByDescending(b => b.CreationDate)
                .Skip(position)
                .Take(resultsPerPage)
                .ToList();
            return nextPage;
        }

        [HttpGet]
        public BlogPostInput? GetPost(int postId)
        {
            return _context.Posts.Where(post => post.BlogPostId == postId).Select(post => new BlogPostInput
            {
                BlogPostId = post.BlogPostId,
                CreationDate = post.CreationDate,
                Title = post.Title,
                Content = post.Content,
                User = post.User
            }).FirstOrDefault();
        }
    }
}