using BlogPlatform.Application.Posts;
using BlogPlatform.Infrastructure.Data;
using BlogPlatform.Infrastructure.Posts;

namespace BlogPlatform.Infrastructure.Tests.Posts;

[Collection("PostgreSqlIntegration")]
public sealed class PostRepositoryPaginationAuthenticatedTests : IClassFixture<PostgreSqlIntegrationTestDatabase>
{
    private readonly PostgreSqlIntegrationTestDatabase _database;

    public PostRepositoryPaginationAuthenticatedTests(PostgreSqlIntegrationTestDatabase database) => _database = database;

    [Fact]
    public async Task ListPublicReadPageAsync_ForAuthenticatedSearch_IncludesOwnedPrivateMatches()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();
        var fixtures = await PaginatedPostSearchTestData.InsertPrivateVisibilityFixturesAsync(_database);

        var repository = new PostgreSqlPostRepository(new NpgsqlConnectionFactory(_database.CreateSettings()));

        var result = await repository.ListPublicReadPageAsync(PostListPageRequest.Create(PaginatedPostSearchTestData.SharedSearchTerm, page: 1, pageSize: 10, requestingUserId: 2));

        Assert.Contains(result.Items, post => post.Id == fixtures.OwnedPrivatePostId);
        Assert.DoesNotContain(result.Items, post => post.Id == fixtures.OtherUsersPrivatePostId);
        Assert.Equal(1, result.TotalCount);
    }

    [Fact]
    public async Task ListPublicReadPageAsync_SearchesByCategoryTitleAndReturnsStablePageSlices()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();

        var repository = new PostgreSqlPostRepository(new NpgsqlConnectionFactory(_database.CreateSettings()));

        var result = await repository.ListPublicReadPageAsync(PostListPageRequest.Create("tEsTiNg", page: 1, pageSize: 1, requestingUserId: 2));

        Assert.Single(result.Items);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal("Using TDD to Drive Interview-Quality Code", result.Items[0].Title);
    }
}
