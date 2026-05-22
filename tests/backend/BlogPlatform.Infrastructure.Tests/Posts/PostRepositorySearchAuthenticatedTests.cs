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
}
