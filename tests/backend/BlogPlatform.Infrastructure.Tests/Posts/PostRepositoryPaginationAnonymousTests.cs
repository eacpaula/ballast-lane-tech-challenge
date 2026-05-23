using BlogPlatform.Application.Posts;
using BlogPlatform.Infrastructure.Data;
using BlogPlatform.Infrastructure.Posts;

namespace BlogPlatform.Infrastructure.Tests.Posts;

[Collection("PostgreSqlIntegration")]
public sealed class PostRepositoryPaginationAnonymousTests : IClassFixture<PostgreSqlIntegrationTestDatabase>
{
    private readonly PostgreSqlIntegrationTestDatabase _database;

    public PostRepositoryPaginationAnonymousTests(PostgreSqlIntegrationTestDatabase database) => _database = database;

    [Fact]
    public async Task ListPublicReadPageAsync_ForAnonymousDefaultListing_ReturnsOnlyRequestedPage()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();

        var repository = new PostgreSqlPostRepository(new NpgsqlConnectionFactory(_database.CreateSettings()));

        var result = await repository.ListPublicReadPageAsync(PostListPageRequest.Create(page: 1, pageSize: 1));

        Assert.Single(result.Items);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal("Building a Lightweight Clean Architecture", result.Items[0].Title);
    }

    [Fact]
    public async Task ListPublicReadPageAsync_ForAnonymousSearch_IsCaseInsensitiveAndExcludesPrivateMatches()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();
        await PaginatedPostSearchTestData.InsertPrivateVisibilityFixturesAsync(_database);

        var repository = new PostgreSqlPostRepository(new NpgsqlConnectionFactory(_database.CreateSettings()));

        var result = await repository.ListPublicReadPageAsync(PostListPageRequest.Create("aRchItEctuRe", page: 1, pageSize: 10));

        Assert.Equal(2, result.Items.Count);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal("Building a Lightweight Clean Architecture", result.Items[0].Title);
    }
}
