using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using TrendingEngine.Data;
using TrendingEngine.Services;

var builder = WebApplication.CreateBuilder(args);

var connStr = builder.Configuration.GetConnectionString("DefaultConnection")!;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connStr));

builder.Services.AddHttpClient("github", client =>
    client.DefaultRequestHeaders.Add("User-Agent", "TrendingEngine/1.0"));

builder.Services.AddHttpClient("reddit", client =>
    client.DefaultRequestHeaders.Add("User-Agent", "TrendingEngine/1.0"));

builder.Services.AddScoped<GithubTrendingService>();
builder.Services.AddScoped<HackerNewsScraperService>();

builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(options =>
        options.UseNpgsqlConnection(connStr)));

builder.Services.AddHangfireServer();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHangfireDashboard("/hangfire");

RecurringJob.AddOrUpdate<GithubTrendingService>(
    "github-trending",
    x => x.RunAsync(),
    Cron.Hourly);

RecurringJob.AddOrUpdate<HackerNewsScraperService>(
    "hackernews-top",
    x => x.RunAsync(),
    "*/30 * * * *");

app.MapControllers();
app.Run();