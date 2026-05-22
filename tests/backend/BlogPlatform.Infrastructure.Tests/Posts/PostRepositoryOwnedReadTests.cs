using BlogPlatform.Infrastructure.Configuration;
using BlogPlatform.Infrastructure.Data;
using BlogPlatform.Infrastructure.Posts;

namespace BlogPlatform.Infrastructure.Tests.Posts;

[Collection("PostgreSqlIntegration")]
public sealed class PostRepositoryOwnedReadTests : IClassFixture<PostgreSqlIntegrationTestDatabase>
{
    private readonly PostgreSqlIntegrationTestDatabase _database;

    public PostRepositoryOwnedReadTests(PostgreSqlIntegrationTestDatabase database) => _database = database;

    [Fact]
    public async Task ListByAuthorAsync_ReturnsOnlyPostsForRequestedAuthor()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();

        var repository = new PostgreSqlPostRepository(
            new NpgsqlConnectionFactory(_database.CreateSettings()));

        var result = await repository.ListByAuthorAsync(2);

        Assert.NotEmpty(result);
        Assert.All(result, p => Assert.Equal(2, p.AuthorUserId));
    }

    [Fact]
    public async Task GetByIdForAuthorAsync_ReturnsNullForDifferentAuthor()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();

        var repository = new PostgreSqlPostRepository(
            new NpgsqlConnectionFactory(_database.CreateSettings()));

        var result = await repository.GetByIdForAuthorAsync(1, 1);

        Assert.Null(result);
    }
}
