using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TrendingEngine.Migrations
{
    /// <inheritdoc />
    public partial class AddTrendingTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GithubRepos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RepoName = table.Column<string>(type: "text", nullable: false),
                    Owner = table.Column<string>(type: "text", nullable: false),
                    Language = table.Column<string>(type: "text", nullable: true),
                    Stars = table.Column<int>(type: "integer", nullable: false),
                    Forks = table.Column<int>(type: "integer", nullable: false),
                    TrendingOn = table.Column<DateOnly>(type: "date", nullable: false),
                    ScrapedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GithubRepos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RedditPosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PostId = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Subreddit = table.Column<string>(type: "text", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    NumComments = table.Column<int>(type: "integer", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: true),
                    Author = table.Column<string>(type: "text", nullable: true),
                    ScrapedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RedditPosts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GithubRepos_Language",
                table: "GithubRepos",
                column: "Language");

            migrationBuilder.CreateIndex(
                name: "IX_GithubRepos_RepoName_TrendingOn",
                table: "GithubRepos",
                columns: new[] { "RepoName", "TrendingOn" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GithubRepos_TrendingOn",
                table: "GithubRepos",
                column: "TrendingOn");

            migrationBuilder.CreateIndex(
                name: "IX_GithubRepos_TrendingOn_Language",
                table: "GithubRepos",
                columns: new[] { "TrendingOn", "Language" });

            migrationBuilder.CreateIndex(
                name: "IX_RedditPosts_PostId",
                table: "RedditPosts",
                column: "PostId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RedditPosts_Score",
                table: "RedditPosts",
                column: "Score");

            migrationBuilder.CreateIndex(
                name: "IX_RedditPosts_ScrapedAt",
                table: "RedditPosts",
                column: "ScrapedAt");

            migrationBuilder.CreateIndex(
                name: "IX_RedditPosts_Subreddit",
                table: "RedditPosts",
                column: "Subreddit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GithubRepos");

            migrationBuilder.DropTable(
                name: "RedditPosts");
        }
    }
}
