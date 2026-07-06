namespace TrendingEngine.Models;

public class RedditPost
{
    public int Id { get; set; }
    public string PostId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Subreddit { get; set; } = string.Empty;
    public int Score { get; set; }
    public int NumComments { get; set; }
    public string? Url { get; set; }
    public string? Author { get; set; }
    public DateTime ScrapedAt { get; set; } = DateTime.UtcNow;
}