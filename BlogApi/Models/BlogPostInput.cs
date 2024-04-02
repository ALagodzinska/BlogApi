using System.ComponentModel.DataAnnotations;

namespace BlogApi.Models
{
    public class BlogPostInput
    {
        [Required]
        public string? Title { get; set; }

        [Required]
        public string? Content { get; set; }

        [Required]
        public string? BackgroundImage { get; set; }

        [Required]
        public string? PreviewImage { get; set; }
        [Required]
        public string? BackgroundImageType { get; set; }
        [Required]
        public string? PreviewImageType { get; set; }

    }
}
