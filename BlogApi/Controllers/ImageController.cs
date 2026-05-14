using Azure.Identity;
using BlogApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;
using static BlogApi.Constants;

namespace BlogApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly ILogger<ImageController> _logger;
        private readonly BloggingContext _context;

        private readonly ImageConversion _imageConversion;

        public ImageController(ILogger<ImageController> logger, BloggingContext context)
        {
            _logger = logger;
            _context = context;
            _imageConversion = new ImageConversion(logger);
        }

        [HttpGet]
        public ActionResult PreviewImage(int postId)
        {
            var post = _context.Posts.Where(post => post.BlogPostId == postId).FirstOrDefault();

            if (post == null)
            {
                return NotFound($"No blog post found with id {postId}");
            }

            return File(post.PreviewImage, post.PreviewImageFormat);
        }

        [HttpGet]
        public ActionResult BackgroundImage(int postId)
        {
            var post = _context.Posts.Where(post => post.BlogPostId == postId).FirstOrDefault();

            if (post == null)
            {
                return NotFound($"No blog post found with id {postId}");
            }
            
            return File(post.BackgroundImage, post.BackgroundImageFormat);
        }

        [HttpGet]
        public ActionResult PreviewImageConversion(int postId)
        {
            var post = _context.Posts.Where(post => post.BlogPostId == postId).FirstOrDefault();

            if (post == null)
            {
                return NotFound($"No blog post found with id {postId}");
            }

            return File(_imageConversion.ImageTransformation(post.PreviewImage, ImageType.Preview), post.PreviewImageFormat);
        }

        [HttpGet]
        public ActionResult BackgroundImageConversion(int postId)
        {
            var post = _context.Posts.Where(post => post.BlogPostId == postId).FirstOrDefault();

            if (post == null)
            {
                return NotFound($"No blog post found with id {postId}");
            }

            return File(_imageConversion.ImageTransformation(post.BackgroundImage, ImageType.Background), post.BackgroundImageFormat);
        }

        [HttpPost]
        public ActionResult Modified(ImageInput image)
        {
            if (image == null || string.IsNullOrWhiteSpace(image.ImageData))
            {
                return BadRequest("Image data is required.");
            }

            byte[] convertedImage;

            try
            {
                convertedImage = Convert.FromBase64String(image.ImageData);
            }
            catch (FormatException)
            {
                return BadRequest("Invalid base64 image data.");
            }

            ImageType imageType = image.ImageType == PreviewImageType ? ImageType.Preview : ImageType.Background;

            return File(_imageConversion.ImageTransformation(convertedImage, imageType), image.ImageFormat);
        }

    }
}
