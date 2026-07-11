using AngleSharp;
using AngleSharp.Html.Parser;
using Microsoft.EntityFrameworkCore;
using TrendingEngine.Data;
using TrendingEngine.Models;

namespace TrendingEngine.Services;

public class GithubTrendingService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AppDbContext _db;
    private readonly ILogger<GithubTrendingService> _logger;

    public GithubTrendingService(
        IHttpClientFactory httpClientFactory,
        AppDbContext db,
        ILogger<GithubTrendingService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _db = db;
        _logger = logger;
    }

    public async Task RunAsync()
    {
        var client = _httpClientFactory.CreateClient("github");
        var html = await client.GetStringAsync("https://github.com/trending");

        var browsingContext = BrowsingContext.New(Configuration.Default);
        var parser = browsingContext.GetService<IHtmlParser>()!;
        var document = await parser.ParseDocumentAsync(html);

        var articles = document.QuerySelectorAll("article.Box-row");
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        int saved = 0;

        foreach (var article in articles)
        {
            var linkEl = article.QuerySelector("h2 a");
            if (linkEl is null) continue;

            var href = linkEl.GetAttribute("href")?.Trim('/') ?? "";
            var parts = href.Split('/');
            if (parts.Length < 2) continue;

            var owner = parts[0];
            var repoName = parts[1];
            var language = article.QuerySelector("[itemprop='programmingLanguage']")?.TextContent.Trim();
            var starsRaw = article.QuerySelector("a[href$='/stargazers']")?.TextContent.Trim() ?? "0";
            var forksRaw = article.QuerySelector("a[href$='/forks']")?.TextContent.Trim() ?? "0";

            var languageParam = string.IsNullOrWhiteSpace(language) ? null : language;
            var stars = ParseNumber(starsRaw);
            var forks = ParseNumber(forksRaw);
            var scrapedAt = DateTime.UtcNow;

            await _db.Database.ExecuteSqlRawAsync(
                "INSERT INTO \"GithubRepos\" (\"RepoName\", \"Owner\", \"Language\", \"Stars\", \"Forks\", \"TrendingOn\", \"ScrapedAt\") " +
                "VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}) " +
                "ON CONFLICT (\"RepoName\", \"TrendingOn\") " +
                "DO UPDATE SET \"Stars\" = EXCLUDED.\"Stars\", \"Forks\" = EXCLUDED.\"Forks\", \"ScrapedAt\" = EXCLUDED.\"ScrapedAt\"",
                repoName, owner, languageParam, stars, forks, today, scrapedAt);

            saved++;
        }

        _logger.LogInformation("GitHub scraper: {Count} repos procesados para {Date}", saved, today);
    }

    private static int ParseNumber(string text)
    {
        text = text.Replace(",", "").Trim();
        if (text.EndsWith("k", StringComparison.OrdinalIgnoreCase))
            return (int)(double.TryParse(text[..^1], out var k) ? k * 1000 : 0);
        return int.TryParse(text, out var n) ? n : 0;
    }
}