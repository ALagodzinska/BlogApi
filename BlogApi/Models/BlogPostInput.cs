using System.ComponentModel.DataAnnotations;

namespace BlogApi.Models
{
    public class BlogPostInput
    {
            public int BlogPostId { get; set; }

            public DateTime CreationDate { get; set; }

            [Required]
            public string? Title { get; set; }

            [Required]
            public string? Content { get; set; }

            public string? BackgroundImage { get; set; }

            public string? PreviewImage { get; set; }

            [Required]
            public string? User { get; set; }
        
    }
}
