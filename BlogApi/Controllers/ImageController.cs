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
        public FileResult PreviewImage(int postId)
        {
            var post = _context.Posts.Where(post => post.BlogPostId == postId).FirstOrDefault();

            if (post != null)
            {
                return File(post.PreviewImage, post.PreviewImageFormat);
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
                return File(post.BackgroundImage, post.BackgroundImageFormat);
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
            return File(_imageConversion.ImageTransformation(post.PreviewImage, ImageType.Preview), post.PreviewImageFormat);
        }

        [HttpGet]
        public ActionResult BackgroundImageConversion(int postId)
        {
            var post = _context.Posts.Where(post => post.BlogPostId == postId).FirstOrDefault();
            if (post == null)
            {
                throw new Exception(String.Format("No blog post found with id {0}", postId));
            }
            return File(_imageConversion.ImageTransformation(post.BackgroundImage, ImageType.Background), post.BackgroundImageFormat);
        }

        //TEST METHOD
        [HttpPost]
        public FileResult Modified(ImageInput image)
        {
            _logger.LogInformation("GOT HERE");
            var convertedImage = Convert.FromBase64String(image.ImageData);
            _logger.LogInformation("COMPARISON {} - {}", image.ImageType, PreviewImageType);
            ImageType imageType = image.ImageType == PreviewImageType ? ImageType.Preview : ImageType.Background;
            _logger.LogInformation("image Type is{}, {}", image.ImageType, imageType);
            return File(_imageConversion.ImageTransformation(convertedImage, imageType), image.ImageFormat);
        }

    }
}
