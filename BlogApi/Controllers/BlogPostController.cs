using BlogApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static BlogApi.Constants;

namespace BlogApi.Controllers
{
    [ApiController]
    [ActivatorUtilitiesConstructor]
    [Route("api/[controller]/[action]")]
    public class BlogPostController : ControllerBase
    {
        private readonly ILogger<BlogPostController> _logger;
        private readonly BloggingContext _context;
        // to get method to resize images
        private readonly ImageConversion _imageConversion;

        private const int ResultsPerPage = 5;

        public BlogPostController(ILogger<BlogPostController> logger, BloggingContext context)
        {
            _logger = logger;
            _context = context;
            _imageConversion = new ImageConversion(logger);
        }

        private IdentityUser GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var user = _context.Users.FirstOrDefault(x => x.Id == userId);
            if (user != null)
            {
                return user;
            }

            throw new Exception("User not found");
        }

        [Authorize]
        [HttpPost]
        public ActionResult<int> PostBlog(BlogPostInput postInput)
        {
            var post = new BlogPost();
            post.Content = postInput.Content;
            post.Title = postInput.Title;
            // Saving modyfied images
            post.PreviewImage = _imageConversion.ImageTransformation(Convert.FromBase64String(postInput.PreviewImage), ImageGroup.Preview, postInput.PreviewImageType);
            post.BackgroundImage = _imageConversion.ImageTransformation(Convert.FromBase64String(postInput.BackgroundImage), ImageGroup.Background, postInput.BackgroundImageType);
            // 
            //post.PreviewImage = Convert.FromBase64String(postInput.PreviewImage);
            //post.BackgroundImage = Convert.FromBase64String(postInput.BackgroundImage);
            post.CreationDate = DateTime.Now;
            post.UserIdentity = GetCurrentUser();
            post.BackgroundImageType = postInput.BackgroundImageType;
            post.PreviewImageType = postInput.PreviewImageType;


            _context.Posts.Add(post);
            _context.SaveChanges();
            return post.BlogPostId;
        }

        [Authorize]
        [HttpPut]
        public IActionResult UpdatePost(BlogPostInput postInput, int postId)
        {
            var entity = _context.Posts.Where(post => post.BlogPostId == postId).FirstOrDefault();
            if (entity == null)
            {
                return NotFound();
            }


            entity.Content = postInput.Content;
            entity.Title = postInput.Title;
            if (postInput.BackgroundImage != null)
            {
                entity.BackgroundImage = _imageConversion.ImageTransformation(Convert.FromBase64String(postInput.BackgroundImage), ImageGroup.Background, postInput.BackgroundImageType);
                entity.BackgroundImageType = postInput.BackgroundImageType;
            }
            if (postInput.PreviewImage != null)
            {
                entity.PreviewImage = _imageConversion.ImageTransformation(Convert.FromBase64String(postInput.PreviewImage), ImageGroup.Preview, postInput.PreviewImageType);
                entity.PreviewImageType = postInput.PreviewImageType;
            }

            _context.SaveChanges();
            return Ok(postId);
        }

        [HttpGet]
        public double GetPageCount()
        {
            return Math.Ceiling(_context.Posts.Count() / (double)ResultsPerPage);
        }

        [HttpGet]
        public List<BlogPostOutput> GetPostsForPage(int page)
        {
            int position = 5 * (page - 1);

            var nextPage = _context.Posts
                .Select(BlogPostOutput.createBlogPostSelector())
                .OrderByDescending(b => b.CreationDate)
                .Skip(position)
                .Take(ResultsPerPage)
                .ToList();
            return nextPage;
        }

        [HttpGet]
        public BlogPostOutput? GetPost(int postId)
        {
            return _context.Posts
                .Where(post => post.BlogPostId == postId)
                .Select(BlogPostOutput.createBlogPostSelector())
                .FirstOrDefault();
        }

        [Authorize]
        [HttpDelete]
        public int DeletePost(int postId) { 
            BlogPost? post = _context.Posts
                .Where(post => post.BlogPostId == postId)
                .FirstOrDefault();
            if (post == null) throw new Exception($"Post with id {postId} doesn't exist.");

            _context.Remove(post);
            _context.SaveChanges();
            return postId;
        }
    }
}