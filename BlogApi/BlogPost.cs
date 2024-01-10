using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace BlogApi
{
    public class BlogPost
    {
        public int BlogPostId { get; set; }

        public DateTime CreationDate { get; set; }

        [Required]
        public string? Title { get; set; }

        [Required]
        public string? Content { get; set; }

        public byte[]? BackgroundImage { get; set; }

        public byte[]? PreviewImage { get; set; }

        [Required]
        public string? User { get; set; }
    }
}
