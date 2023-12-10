namespace BlogApi
{
    public class BlogPost
    {
        public int BlogPostId { get; set; }

        public DateTime CreationDate { get; set; }

        public string? Title { get; set; }

        public string? Content { get; set; }

        public string? Image { get; set; }

        public string? User { get; set; }
    }
}
