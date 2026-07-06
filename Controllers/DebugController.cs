using Microsoft.AspNetCore.Mvc;
using TrendingEngine.Services;

namespace TrendingEngine.Controllers;

[ApiController]
[Route("debug")]
public class DebugController : ControllerBase
{
    private readonly GithubTrendingService _github;
    private readonly RedditScraperService _reddit;

    public DebugController(GithubTrendingService github, RedditScraperService reddit)
    {
        _github = github;
        _reddit = reddit;
    }

    [HttpGet("scrape-github")]
    public async Task<IActionResult> ScrapeGithub()
    {
        await _github.RunAsync();
        return Ok("GitHub scraper ejecutado. Revisa la tabla GithubRepos.");
    }

    [HttpGet("scrape-reddit")]
    public async Task<IActionResult> ScrapeReddit()
    {
        await _reddit.RunAsync();
        return Ok("Reddit scraper ejecutado. Revisa la tabla RedditPosts.");
    }
}