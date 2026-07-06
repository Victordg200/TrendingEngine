using Microsoft.EntityFrameworkCore;
using TrendingEngine.Models;

namespace TrendingEngine.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<GithubRepo> GithubRepos => Set<GithubRepo>();
    public DbSet<RedditPost> RedditPosts => Set<RedditPost>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<GithubRepo>(entity =>
        {
            entity.HasIndex(e => e.TrendingOn);
            entity.HasIndex(e => e.Language);
            entity.HasIndex(e => new { e.TrendingOn, e.Language });
            entity.HasIndex(e => new { e.RepoName, e.TrendingOn }).IsUnique();
        });

        modelBuilder.Entity<RedditPost>(entity =>
        {
            entity.HasIndex(e => e.PostId).IsUnique();
            entity.HasIndex(e => e.Subreddit);
            entity.HasIndex(e => e.Score);
            entity.HasIndex(e => e.ScrapedAt);
        });
    }
}