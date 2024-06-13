using BlogApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private const int DeletedPostsResultsPerPage = 10;
        private const int LatestPostsResults = 4;

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
            post.PreviewImage = _imageConversion.ImageTransformation(Convert.FromBase64String(postInput.PreviewImage), ImageType.Preview);
            post.BackgroundImage = _imageConversion.ImageTransformation(Convert.FromBase64String(postInput.BackgroundImage), ImageType.Background);
            post.CreationDate = DateTime.Now;
            post.UserIdentity = GetCurrentUser();
            post.BackgroundImageFormat = postInput.BackgroundImageFormat;
            post.PreviewImageFormat = postInput.PreviewImageFormat;
            _context.Posts.Add(post);
            _context.SaveChanges();
            return post.BlogPostId;
        }

        [Authorize]
        [HttpPut]
        public IActionResult UpdatePost(BlogPostInput postInput, int postId)
        {
            _logger.LogInformation("HERE WE ARE IN BACKEND -- {}", postId);
            var entity = _context.Posts.Where(post => post.BlogPostId == postId).FirstOrDefault();
            if (entity == null)
            {
                return NotFound();
            }


            entity.Content = postInput.Content;
            entity.Title = postInput.Title;
            if (postInput.BackgroundImage != null && postInput.BackgroundImageFormat != null)
            {
                entity.BackgroundImage = _imageConversion.ImageTransformation(Convert.FromBase64String(postInput.BackgroundImage), ImageType.Background);
                entity.BackgroundImageFormat = postInput.BackgroundImageFormat;
            }
            if (postInput.PreviewImage != null && postInput.PreviewImageFormat != null)
            {
                entity.PreviewImage = _imageConversion.ImageTransformation(Convert.FromBase64String(postInput.PreviewImage), ImageType.Preview);
                entity.PreviewImageFormat = postInput.PreviewImageFormat;
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
            int position = ResultsPerPage * (page - 1);

            var nextPage = _context.Posts
                .Include(post => post.UserIdentity)
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
                .Include(post => post.UserIdentity)
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

        [Authorize]
        [HttpGet]
        public double GetDeletedPostsPageCount()
        {
            int deletedPostsCount = _context.Posts.IgnoreQueryFilters()
                .Where(post => post.IsDeleted == true)
                .Count();
            return Math.Ceiling( deletedPostsCount / (double)DeletedPostsResultsPerPage);
        }

        [Authorize]
        [HttpGet]
        public List<DeletedBlogPostOutput> GetDeletedPostsPerPage(int page)
        {
            int position = DeletedPostsResultsPerPage * (page - 1);

            var nextPage = _context.Posts
                .IgnoreQueryFilters()
                .Where(post => post.IsDeleted == true)
                .Select(DeletedBlogPostOutput.createBlogPostSelector())
                .OrderByDescending(b => b.CreationDate)
                .Skip(position)
                .Take(DeletedPostsResultsPerPage)
                .ToList();
            return nextPage;
        }

        [Authorize]
        [HttpPut]
        public IActionResult RestorePost(int postId)
        {
            var postToRestore = _context.Posts
                .IgnoreQueryFilters()
                .Where(post => post.BlogPostId == postId).FirstOrDefault();

            if (postToRestore == null)
            {
                return NotFound();
            }

            postToRestore.IsDeleted = false;
            postToRestore.DeletedAt = null;

            _context.SaveChanges();
            return Ok(postId);
        }

        [Authorize]
        [HttpPut]
        public IActionResult FeaturePost(int postId)
        {
            var postToFeature = _context.Posts
                .Where(post => post.BlogPostId == postId).FirstOrDefault();

            if (postToFeature == null)
            {
                return NotFound();
            }

            var oldFeaturedPosts = _context.Posts.Where(post => post.IsFeatured == true);
            if(oldFeaturedPosts.Any()) {
                foreach (var post in oldFeaturedPosts)
                {
                    post.IsFeatured = false;
                }
            }

            postToFeature.IsFeatured = true;
            _context.SaveChanges();
            return Ok(postId);
        }

        [HttpGet]
        public BlogPostOutput? GetFeaturedPost()
        {
            return _context.Posts
                .Where(post => post.IsFeatured == true)
                .Include(post => post.UserIdentity)
                .Select(BlogPostOutput.createBlogPostSelector())
                .FirstOrDefault();
        }

        [HttpGet]
        public List<BlogPostOutput> GetLatestPosts()
        {

            var latestPosts = _context.Posts
                .Where(post => post.IsFeatured == false)
                .Include(post => post.UserIdentity)
                .Select(BlogPostOutput.createBlogPostSelector())
                .OrderByDescending(b => b.CreationDate)
                .Take(LatestPostsResults)
                .ToList();
            return latestPosts;
        }

    }
}