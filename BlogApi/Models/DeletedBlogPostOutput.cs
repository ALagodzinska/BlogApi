namespace BlogApi.Models
{
    public class DeletedBlogPostOutput
    {
        private const string DefaultUser = "Anonymous";
        public DeletedBlogPostOutput(BlogPost entity)
        {
            BlogPostId = entity.BlogPostId;
            CreationDate = entity.CreationDate;
            Title = entity.Title;
            User = entity.UserIdentity != null ? entity.UserIdentity.UserName : DefaultUser;
            DeletedAt = entity.DeletedAt;
        }

        static public Func<BlogPost, DeletedBlogPostOutput> createBlogPostSelector()
        {
            return post => new DeletedBlogPostOutput(post);
        }

        public int BlogPostId { get; set; }

        public DateTime CreationDate { get; set; }

        public string? Title { get; set; }

        public string? User { get; set; }

        public DateTimeOffset? DeletedAt { get; set; }
    }
}
