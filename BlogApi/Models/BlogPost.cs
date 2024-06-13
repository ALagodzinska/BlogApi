using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace BlogApi.Models
{
    public class BlogPost: ISoftDelete
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
        public string? BackgroundImageFormat { get; set; }

        [Required]
        public byte[]? PreviewImage { get; set; }

        [Required]
        public string? PreviewImageFormat { get; set; }

        public IdentityUser UserIdentity { get; set; }

        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }

        [DefaultValue(false)]
        public bool IsFeatured { get; set; }
    }
}
