using BlogPlatform.Infrastructure.Data;
using BlogPlatform.Infrastructure.Posts;

namespace BlogPlatform.Infrastructure.Tests.Posts;

[Collection("PostgreSqlIntegration")]
public sealed class PostRepositorySearchAnonymousTests : IClassFixture<PostgreSqlIntegrationTestDatabase>
{
    private readonly PostgreSqlIntegrationTestDatabase _database;

    public PostRepositorySearchAnonymousTests(PostgreSqlIntegrationTestDatabase database) => _database = database;

    [Fact]
    public async Task SearchPublicReadAsync_ForAnonymousSearch_ReturnsOnlyPublicAvailableMatches()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();
        await SearchPostTestData.InsertPrivateVisibilityFixturesAsync(_database);

        var repository = new PostgreSqlPostRepository(new NpgsqlConnectionFactory(_database.CreateSettings()));

        var result = await repository.SearchPublicReadAsync(SearchPostTestData.SharedSearchTerm, null);

        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchPublicReadAsync_IsCaseInsensitiveForPublicMatches()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();

        var repository = new PostgreSqlPostRepository(new NpgsqlConnectionFactory(_database.CreateSettings()));

        var result = await repository.SearchPublicReadAsync("aRchItEctuRe", null);

        Assert.NotEmpty(result);
        Assert.Contains(result, r => r.Title == "Building a Lightweight Clean Architecture");
    }

    [Fact]
    public async Task SearchPublicReadAsync_AnonymousSearch_ExcludesScheduledPost()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();

        var repository = new PostgreSqlPostRepository(new NpgsqlConnectionFactory(_database.CreateSettings()));

        var result = await repository.SearchPublicReadAsync("Advanced Repository Testing", null);

        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchPublicReadAsync_AnonymousSearch_ExcludesExpiredPost()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();

        var repository = new PostgreSqlPostRepository(new NpgsqlConnectionFactory(_database.CreateSettings()));

        var result = await repository.SearchPublicReadAsync("Archived", null);

        Assert.Empty(result);
    }
}
