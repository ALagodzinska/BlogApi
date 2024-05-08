using static BlogApi.Constants;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using BlogApi.Controllers;

namespace BlogApi
{
    public class ImageConversion
    {
        ILogger _logger;
        public ImageConversion(ILogger logger)
        {
            _logger = logger;
        }
        public byte[] ImageTransformation(byte[] imageFromPost, ImageGroup imageGroup, string imageType)
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
                    ? CalculatePreviewImageSize(image.Height, image.Width) : CalculateBackgroundImageSize(image.Height, image.Width);
                _logger.LogInformation("Width - {}, Height -{} ", resizeWidth, resizeHeight);
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
                // TODO: Change here image format on the right one
                croppedImage.Save(writeStream, ImageFormat.Jpeg);
                return writeStream.ToArray();
            }
        }

        public static (int height, int width) CalculateBackgroundImageSize(int imageHeight, int imageWidth)
        {
            int width = Constants.BackgroundImageWidth;
            int height = (width * imageHeight) / imageWidth;
            if(height < Constants.BackgroundImageHeight)
            {
                height = Constants.BackgroundImageHeight;
                width = (height * imageWidth) / imageHeight;
            }

            return (height, width);
        }

        public static (int height, int width) CalculatePreviewImageSize(int imageHeight, int imageWidth)
        {
            int height = Constants.PreviewImageHeight;
            int width = (height * imageWidth) / imageHeight;
            if (width < Constants.PreviewImageWidth)
            {
                width = Constants.PreviewImageWidth;
                height = (width * imageHeight) / imageWidth;
            }

            return (height, width);
        }

        private static Bitmap ResizeImage(Image image, int width, int height)
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
