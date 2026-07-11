using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TrendingEngine.Data;

namespace TrendingEngine.Services;

public class HackerNewsScraperService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AppDbContext _db;
    private readonly ILogger<HackerNewsScraperService> _logger;

    public HackerNewsScraperService(
        IHttpClientFactory httpClientFactory,
        AppDbContext db,
        ILogger<HackerNewsScraperService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _db = db;
        _logger = logger;
    }

    public async Task RunAsync()
    {
        var client = _httpClientFactory.CreateClient("reddit");

        var idsJson = await client.GetStringAsync(
            "https://hacker-news.firebaseio.com/v0/topstories.json");

        var ids = JsonSerializer.Deserialize<int[]>(idsJson)!.Take(30);
        int saved = 0;

        foreach (var id in ids)
        {
            var itemJson = await client.GetStringAsync(
                $"https://hacker-news.firebaseio.com/v0/item/{id}.json");

            using var doc = JsonDocument.Parse(itemJson);
            var root = doc.RootElement;

            if (!root.TryGetProperty("title", out var titleEl)) continue;

            var postId = id.ToString();
            var title = titleEl.GetString() ?? "";
            var score = root.TryGetProperty("score", out var s) ? s.GetInt32() : 0;
            var comments = root.TryGetProperty("descendants", out var d) ? d.GetInt32() : 0;
            var url = root.TryGetProperty("url", out var u) ? u.GetString() : null;
            var author = root.TryGetProperty("by", out var b) ? b.GetString() : null;
            var scrapedAt = DateTime.UtcNow;

            await _db.Database.ExecuteSqlRawAsync(
                "INSERT INTO \"RedditPosts\" (\"PostId\", \"Title\", \"Subreddit\", \"Score\", \"NumComments\", \"Url\", \"Author\", \"ScrapedAt\") " +
                "VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}) " +
                "ON CONFLICT (\"PostId\") DO UPDATE SET " +
                "\"Score\" = EXCLUDED.\"Score\", " +
                "\"NumComments\" = EXCLUDED.\"NumComments\", " +
                "\"ScrapedAt\" = EXCLUDED.\"ScrapedAt\"",
                postId, title, "hackernews", score, comments, url, author, scrapedAt);

            saved++;
        }

        _logger.LogInformation("HackerNews scraper: {Count} posts procesados", saved);
    }
}