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
            return ImageTransformation(post.PreviewImage, ImageGroup.Preview, post.PreviewImageType);
        }

        [HttpGet]
        public ActionResult BackgroundImageConversion(int postId)
        {
            var post = _context.Posts.Where(post => post.BlogPostId == postId).FirstOrDefault();
            if (post == null)
            {
                throw new Exception(String.Format("No blog post found with id {0}", postId));
            }
            return ImageTransformation(post.BackgroundImage, ImageGroup.Background, post.BackgroundImageType);
        }

        private ActionResult ImageTransformation(byte[] imageFromPost, ImageGroup imageGroup, string imageType)
        {
            if (imageFromPost == null)
            {
                throw new Exception(String.Format("No image found"));
            }
            Image image;
            Bitmap resizedImage;
            Bitmap croppedImage;
            using (MemoryStream readStream = new(imageFromPost))
            {
                image = Image.FromStream(readStream);

                // Getting values for ratio resize -> width and height for specific image group
                (int resizeHeight, int resizeWidth) = imageGroup == ImageGroup.Preview 
                    ? GetPreviewImageResizeValues(image) : GetBackgroundImageResizeValues(image);
                // Creating resized image
                resizedImage = ResizeImage(image, resizeWidth, resizeHeight);
                // Image final size by image group type
                (int finalImageWidth, int finalImageHeight) = imageGroup == ImageGroup.Preview
                    ? (Constants.PreviewImageWidth, Constants.PreviewImageHeight)
                    : (Constants.BackgroundImageWidth, Constants.BackgroundImageHeight);
                // Cropping image from center
                croppedImage = CropImageFromCenter((Image)resizedImage, finalImageWidth, finalImageHeight);
            }

            using (MemoryStream writeStream = new())
            {
                croppedImage.Save(writeStream, ImageFormat.Jpeg);                
                return File(writeStream.ToArray(), imageType);
            }
        }
        
        private (int height, int width) GetPreviewImageResizeValues(Image image)
        {
            int height;
            int width;
            if(image.Height > image.Width)
            {
                width = Constants.PreviewImageWidth;
                height = (image.Height * width) /image.Width;
            }
            else
            {
                height = Constants.PreviewImageHeight;
                width = (image.Width * height) / image.Height;
            }

            return (height, width);
        }

        private (int height, int width) GetBackgroundImageResizeValues(Image image)
        {
            int height;
            int width;
            if (image.Width > image.Height)
            {
                width = Constants.BackgroundImageWidth;
                height = (image.Height * width) / image.Width;                
                
            }
            else
            {
                height = Constants.BackgroundImageHeight;
                width = (image.Width * height) / image.Height;
            }
            return (height, width);
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);            
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private Bitmap CropImageFromCenter(Image img, int width, int height)
        {
            // Calculate the crop area centered in the original image
            int x = (img.Width - width) / 2;
            int y = (img.Height - height) / 2;

            Rectangle cropRect = new Rectangle(x, y, width, height);

            Bitmap croppedImage = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(croppedImage))
            {
                g.DrawImage(img, new Rectangle(0, 0, width, height), cropRect, GraphicsUnit.Pixel);
            }
            return croppedImage;
        }
    }
}
