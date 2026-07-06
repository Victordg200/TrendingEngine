using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrendingEngine.Data;
using TrendingEngine.Models;

namespace TrendingEngine.Controllers;

[ApiController]
[Route("api/trending")]
public class TrendingController : ControllerBase
{
    private readonly AppDbContext _db;

    public TrendingController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("github")]
    public async Task<ActionResult<PaginatedResponse<GithubRepo>>> GetGithub(
        [FromQuery] string? language,
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to,
        [FromQuery] int page = 1,
        [FromQuery] int size = 20)
    {
        var query = _db.GithubRepos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(language))
            query = query.Where(r => r.Language == language);

        if (from.HasValue)
            query = query.Where(r => r.TrendingOn >= from.Value);

        if (to.HasValue)
            query = query.Where(r => r.TrendingOn <= to.Value);

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(r => r.Stars)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return Ok(new PaginatedResponse<GithubRepo>
        {
            Items = items,
            Total = total,
            Page = page,
            PageSize = size
        });
    }

    [HttpGet("hackernews")]
    public async Task<ActionResult<PaginatedResponse<RedditPost>>> GetHackernews(
        [FromQuery] int? minScore,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int page = 1,
        [FromQuery] int size = 20)
    {
        var query = _db.RedditPosts.AsQueryable();

        if (minScore.HasValue)
            query = query.Where(p => p.Score >= minScore.Value);

        if (from.HasValue)
            query = query.Where(p => p.ScrapedAt >= from.Value);

        if (to.HasValue)
            query = query.Where(p => p.ScrapedAt <= to.Value);

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(p => p.Score)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return Ok(new PaginatedResponse<RedditPost>
        {
            Items = items,
            Total = total,
            Page = page,
            PageSize = size
        });
    }
}