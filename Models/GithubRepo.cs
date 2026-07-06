namespace TrendingEngine.Models;

public class GithubRepo
{
    public int Id { get; set; }
    public string RepoName { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
    public string? Language { get; set; }
    public int Stars { get; set; }
    public int Forks { get; set; }
    public DateOnly TrendingOn { get; set; }
    public DateTime ScrapedAt { get; set; } = DateTime.UtcNow;
}