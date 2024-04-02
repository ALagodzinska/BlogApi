using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace BlogApi.Models
{
    public enum ImageType
    {
        JPEG,
        PNG
    }

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
        public ImageType? BackgroundImageType { get; set; }

        [Required]
        public byte[]? PreviewImage { get; set; }

        [Required]
        public ImageType? PreviewImageType { get; set; }

        public IdentityUser UserIdentity { get; set; }

        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
    }
}
