using BlogPlatform.Domain.Posts;
using BlogPlatform.Infrastructure.Configuration;
using BlogPlatform.Infrastructure.Data;
using BlogPlatform.Infrastructure.Posts;

namespace BlogPlatform.Infrastructure.Tests.Posts;

[Collection("PostgreSqlIntegration")]
public sealed class PostRepositoryVisibilityTests : IClassFixture<PostgreSqlIntegrationTestDatabase>
{
    private readonly PostgreSqlIntegrationTestDatabase _database;

    public PostRepositoryVisibilityTests(PostgreSqlIntegrationTestDatabase database) => _database = database;

    [Fact]
    public async Task ListPublicReadAsync_ExcludesScheduledPost()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();

        var result = await CreateRepository().ListPublicReadAsync();

        Assert.DoesNotContain(result, p => p.Title == "Upcoming: Advanced Repository Testing Patterns");
    }

    [Fact]
    public async Task ListPublicReadAsync_ExcludesExpiredPost()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();

        var result = await CreateRepository().ListPublicReadAsync();

        Assert.DoesNotContain(result, p => p.Title == "Archived: Early Design Decisions");
    }

    [Fact]
    public async Task ListPublicReadAsync_IncludesPostWithNoDates()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();

        var result = await CreateRepository().ListPublicReadAsync();

        Assert.Contains(result, p => p.Title == "Clean Architecture Without the Ceremony");
    }

    [Fact]
    public async Task ListPublicReadAsync_IncludesPostWithActiveDateWindow()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();

        var authorId = await _database.GetUserIdByUsernameAsync("user");
        var categoryId = await _database.GetCategoryIdByTitleAsync("Architecture");
        var repo = CreateRepository();

        var active = BlogPost.Create(
            authorUserId: authorId,
            categoryId: categoryId,
            title: "Active Window Post",
            summary: null,
            content: "content",
            publishDate: DateTimeOffset.UtcNow.AddDays(-1),
            expirationDate: DateTimeOffset.UtcNow.AddDays(7));

        await repo.CreateAsync(active);

        var result = await repo.ListPublicReadAsync();

        Assert.Contains(result, p => p.Title == "Active Window Post");
    }

    [Fact]
    public async Task GetPublicReadByIdAsync_ReturnsNullForScheduledPost()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();

        var authorId = await _database.GetUserIdByUsernameAsync("user");
        var categoryId = await _database.GetCategoryIdByTitleAsync("Architecture");
        var repo = CreateRepository();

        var scheduled = await repo.CreateAsync(BlogPost.Create(
            authorUserId: authorId,
            categoryId: categoryId,
            title: "Scheduled For Visibility Test",
            summary: null,
            content: "content",
            publishDate: DateTimeOffset.UtcNow.AddDays(7)));

        var result = await repo.GetPublicReadByIdAsync(scheduled.Id);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetPublicReadByIdAsync_ReturnsNullForExpiredPost()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();

        var authorId = await _database.GetUserIdByUsernameAsync("user");
        var categoryId = await _database.GetCategoryIdByTitleAsync("Architecture");
        var repo = CreateRepository();

        var expired = await repo.CreateAsync(BlogPost.Create(
            authorUserId: authorId,
            categoryId: categoryId,
            title: "Expired For Visibility Test",
            summary: null,
            content: "content",
            publishDate: DateTimeOffset.UtcNow.AddDays(-30),
            expirationDate: DateTimeOffset.UtcNow.AddDays(-1)));

        var result = await repo.GetPublicReadByIdAsync(expired.Id);

        Assert.Null(result);
    }

    private PostgreSqlPostRepository CreateRepository() =>
        new(new NpgsqlConnectionFactory(_database.CreateSettings()));
}
