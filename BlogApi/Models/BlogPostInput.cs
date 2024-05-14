using System.ComponentModel.DataAnnotations;

namespace BlogApi.Models
{
    public class BlogPostInput
    {
        [Required]
        public string? Title { get; set; }

        [Required]
        public string? Content { get; set; }

        public string? BackgroundImage { get; set; }

        public string? PreviewImage { get; set; }
        public string? BackgroundImageFormat { get; set; }
        
        public string? PreviewImageFormat { get; set; }

    }
}
