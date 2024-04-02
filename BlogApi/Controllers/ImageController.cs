using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

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

        [HttpGet]
        public ActionResult PreviewImageExperementation(int postId)
        {
            var post = _context.Posts.Where(post => post.BlogPostId == postId).FirstOrDefault();
            if (post == null) 
            {
                throw new Exception(String.Format("No blog post found with id {0}", postId)); 
            }
            if (post.PreviewImage == null)
            {
                throw new Exception(String.Format("No preview image for post with id {0}", postId));
            }

            Image image;
            Bitmap resizedImage;
            Bitmap croppedImage;
            using (MemoryStream readStream = new(post.PreviewImage))
            {                    
                image = Image.FromStream(readStream);

                // Getting values for ratio resize -> width and height
                var resizedSize = GetPreviewImageResizeValues(image);
                // Creating resized image
                resizedImage = ResizeImage(image, resizedSize.width, resizedSize.height);
                // Cropping image from center
                croppedImage = CropImageFromCenter((Image)resizedImage, Constants.PreviewImageWidth, Constants.PreviewImageHeight);
            }

            using (MemoryStream writeStream = new())
            {
                croppedImage.Save(writeStream, ImageFormat.Jpeg);
                return File(writeStream.ToArray(), "image/jpeg");
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
            _logger.LogInformation("Image width {}, Image height {}", img.Width, img.Height);
            _logger.LogInformation("Width {width}, height {height}, x {x}, y {y}", width, height, x, y);

            Bitmap croppedImage = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(croppedImage))
            {
                g.DrawImage(img, new Rectangle(0, 0, width, height), cropRect, GraphicsUnit.Pixel);
            }
            return croppedImage;
        }
    }
}
