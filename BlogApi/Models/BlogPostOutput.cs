using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;

namespace BlogApi.Models
{
    public class BlogPostOutput
    {
        private const string DefaultUser = "Anonymous";

        public BlogPostOutput(BlogPost entity)
        {
            BlogPostId = entity.BlogPostId;
            CreationDate = entity.CreationDate;
            Title = entity.Title;
            Content = entity.Content;
            PreviewImageFormat = entity.PreviewImageFormat;
            BackgroundImageFormat = entity.BackgroundImageFormat;
            User = entity.UserIdentity != null ? entity.UserIdentity.UserName : DefaultUser;
        }
        
        static public Func<BlogPost, BlogPostOutput> createBlogPostSelector()
        {
            return post => new BlogPostOutput(post);
        }

        public int BlogPostId { get; set; }

        public DateTime CreationDate { get; set; }

        public string? Title { get; set; }

        public string? Content { get; set; }

        public string? BackgroundImage { get; set; }

        public string? PreviewImage { get; set; }
        public string? User { get; set; }
        public string? BackgroundImageFormat { get; set; }
        public string? PreviewImageFormat { get; set; }
    }
}
