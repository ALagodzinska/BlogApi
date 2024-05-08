using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using static BlogApi.Constants;

namespace BlogApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly ILogger<BlogPostController> _logger;
        private readonly BloggingContext _context;

        private readonly ImageConversion _imageConversion;

        public ImageController(ILogger<BlogPostController> logger, BloggingContext context)
        {
            _logger = logger;
            _context = context;
            _imageConversion = new ImageConversion(logger);
        }

        [HttpGet]
        public FileResult PreviewImage(int postId)
        {
            var post = _context.Posts.Where(post => post.BlogPostId == postId).FirstOrDefault();

            if (post != null)
            {
                return File(post.PreviewImage, post.PreviewImageType);
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
                return File(post.BackgroundImage, post.BackgroundImageType);
            }
            else
            {
                throw new Exception(String.Format("No blog post found with id {0}", postId));
            }
        }

        [HttpGet]
        public ActionResult PreviewImageConversion(int postId)
        {
            var post = _context.Posts.Where(post => post.BlogPostId == postId).FirstOrDefault();
            if (post == null)
            {
                throw new Exception(String.Format("No blog post found with id {0}", postId));
            }
            return File(_imageConversion.ImageTransformation(post.PreviewImage, ImageGroup.Preview, post.PreviewImageType), post.PreviewImageType);
        }

        [HttpGet]
        public ActionResult BackgroundImageConversion(int postId)
        {
            var post = _context.Posts.Where(post => post.BlogPostId == postId).FirstOrDefault();
            if (post == null)
            {
                throw new Exception(String.Format("No blog post found with id {0}", postId));
            }
            return File(_imageConversion.ImageTransformation(post.BackgroundImage, ImageGroup.Background, post.BackgroundImageType), post.BackgroundImageType);
        }


    }
}
