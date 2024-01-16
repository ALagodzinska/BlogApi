using BlogApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogApi
{
    public class BloggingContext : DbContext
    {
        public BloggingContext(DbContextOptions<BloggingContext> options)
       : base(options)
        {
        }

        public DbSet<BlogPost> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=BlogWebsiteDb");
    }
}
