using BlogApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogApi
{
    public class BloggingContext : IdentityDbContext
    {
        public BloggingContext(DbContextOptions<BloggingContext> options)
       : base(options)
        {
        }

        public DbSet<BlogPost> Posts { get; set; }

        public DbSet<IdentityUser> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options
            .UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=BlogWebsiteDb")
            .AddInterceptors(new SoftDeleteInterceptor());

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Automatically adding query filter to 
            // all LINQ queries that use BlogPost
            modelBuilder.Entity<BlogPost>()
                .HasQueryFilter(x => x.IsDeleted == false);
            base.OnModelCreating(modelBuilder);         
        }
    }
}
