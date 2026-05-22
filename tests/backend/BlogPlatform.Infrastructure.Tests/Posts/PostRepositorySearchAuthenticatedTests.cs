using BlogPlatform.Infrastructure.Data;
using BlogPlatform.Infrastructure.Posts;

namespace BlogPlatform.Infrastructure.Tests.Posts;

[Collection("PostgreSqlIntegration")]
public sealed class PostRepositorySearchAuthenticatedTests : IClassFixture<PostgreSqlIntegrationTestDatabase>
{
    private readonly PostgreSqlIntegrationTestDatabase _database;

    public PostRepositorySearchAuthenticatedTests(PostgreSqlIntegrationTestDatabase database) => _database = database;

    [Fact]
    public async Task SearchPublicReadAsync_ForAuthenticatedSearch_IncludesOwnedPrivateMatches()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();
        var fixtures = await SearchPostTestData.InsertPrivateVisibilityFixturesAsync(_database);

        var repository = new PostgreSqlPostRepository(new NpgsqlConnectionFactory(_database.CreateSettings()));

        var result = await repository.SearchPublicReadAsync(SearchPostTestData.SharedSearchTerm, 2);

        Assert.Contains(result, post => post.Id == fixtures.OwnedPrivatePostId);
        Assert.DoesNotContain(result, post => post.Id == fixtures.OtherUsersPrivatePostId);
    }

    [Fact]
    public async Task SearchPublicReadAsync_SearchesByCategoryTitle()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();

        var repository = new PostgreSqlPostRepository(new NpgsqlConnectionFactory(_database.CreateSettings()));

        var result = await repository.SearchPublicReadAsync("testing", 2);

        Assert.Contains(result, post => post.Title == "Using TDD to Drive Interview-Quality Code");
    }

    [Fact]
    public async Task SearchPublicReadAsync_AuthenticatedSearch_IncludesOwnScheduledPost()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();

        var authorId = await _database.GetUserIdByUsernameAsync("user");
        var categoryId = await _database.GetCategoryIdByTitleAsync("Architecture");
        var repository = new PostgreSqlPostRepository(new NpgsqlConnectionFactory(_database.CreateSettings()));

        var scheduled = await repository.CreateAsync(BlogPlatform.Domain.Posts.BlogPost.Create(
            authorUserId: authorId,
            categoryId: categoryId,
            title: "My Scheduled Search Test Post",
            summary: null,
            content: "content",
            publishDate: DateTimeOffset.UtcNow.AddDays(7)));

        var result = await repository.SearchPublicReadAsync("Scheduled Search Test", authorId);

        Assert.Contains(result, p => p.Id == scheduled.Id);
    }

    [Fact]
    public async Task SearchPublicReadAsync_AuthenticatedSearch_ExcludesOtherUsersScheduledPost()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();

        var authorId = await _database.GetUserIdByUsernameAsync("user");
        var adminId = await _database.GetUserIdByUsernameAsync("admin");
        var categoryId = await _database.GetCategoryIdByTitleAsync("Architecture");
        var repository = new PostgreSqlPostRepository(new NpgsqlConnectionFactory(_database.CreateSettings()));

        var scheduled = await repository.CreateAsync(BlogPlatform.Domain.Posts.BlogPost.Create(
            authorUserId: authorId,
            categoryId: categoryId,
            title: "Others Scheduled Cross User Test",
            summary: null,
            content: "content",
            publishDate: DateTimeOffset.UtcNow.AddDays(7)));

        var result = await repository.SearchPublicReadAsync("Cross User Test", adminId);

        Assert.DoesNotContain(result, p => p.Id == scheduled.Id);
    }
}
