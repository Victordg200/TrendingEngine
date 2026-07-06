using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrendingEngine.Data;

namespace TrendingEngine.Controllers;

[ApiController]
[Route("api/stats")]
public class StatsController : ControllerBase
{
    private readonly AppDbContext _db;

    public StatsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("top-languages")]
    public async Task<IActionResult> TopLanguages([FromQuery] int weeks = 4)
    {
        var from = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-weeks * 7));

        var result = await _db.GithubRepos
            .Where(r => r.TrendingOn >= from && r.Language != null)
            .GroupBy(r => r.Language!)
            .Select(g => new
            {
                Language = g.Key,
                Count = g.Count(),
                AvgStars = (int)g.Average(r => r.Stars)
            })
            .OrderByDescending(x => x.Count)
            .Take(10)
            .ToListAsync();

        return Ok(result);
    }

    [HttpGet("trending-history")]
    public async Task<IActionResult> TrendingHistory([FromQuery] string lang = "Python")
    {
        var result = await _db.GithubRepos
            .Where(r => r.Language == lang)
            .GroupBy(r => r.TrendingOn)
            .Select(g => new
            {
                Date = g.Key,
                RepoCount = g.Count(),
                AvgStars = (int)g.Average(r => r.Stars)
            })
            .OrderBy(x => x.Date)
            .ToListAsync();

        return Ok(result);
    }

    [HttpGet("summary")]
    public async Task<IActionResult> Summary()
    {
        var totalGithub = await _db.GithubRepos.CountAsync();
        var totalHackernews = await _db.RedditPosts.CountAsync();
        var uniqueRepos = await _db.GithubRepos.Select(r => r.RepoName).Distinct().CountAsync();
        var lastGithub = await _db.GithubRepos.MaxAsync(r => r.ScrapedAt);
        var lastHackernews = await _db.RedditPosts.MaxAsync(p => p.ScrapedAt);

        return Ok(new
        {
            TotalGithubRecords = totalGithub,
            TotalHackernewsRecords = totalHackernews,
            UniqueRepos = uniqueRepos,
            LastGithubScrape = lastGithub,
            LastHackerNewsScrape = lastHackernews
        });
    }
}