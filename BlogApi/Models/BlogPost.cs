using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace BlogApi.Models
{
    public class BlogPost
    {
        public int BlogPostId { get; set; }

        public DateTime CreationDate { get; set; }

        [Required]
        public string? Title { get; set; }

        [Required]
        public string? Content { get; set; }

        [Required]
        public byte[]? BackgroundImage { get; set; }

        [Required]
        public byte[]? PreviewImage { get; set; }

        [Required]
        public string? User { get; set; }
    }
}
