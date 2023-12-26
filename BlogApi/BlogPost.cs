using System.ComponentModel.DataAnnotations;

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

        [Required]
        public string? Image { get; set; }

        [Required]
        public string? User { get; set; }
    }
}
