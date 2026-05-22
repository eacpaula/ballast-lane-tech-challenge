using BlogPlatform.Domain.Posts;
using BlogPlatform.Infrastructure.Configuration;
using BlogPlatform.Infrastructure.Data;
using BlogPlatform.Infrastructure.Posts;

namespace BlogPlatform.Infrastructure.Tests.Posts;

[Collection("PostgreSqlIntegration")]
public sealed class PostRepositoryDatePersistenceTests : IClassFixture<PostgreSqlIntegrationTestDatabase>
{
    private readonly PostgreSqlIntegrationTestDatabase _database;

    public PostRepositoryDatePersistenceTests(PostgreSqlIntegrationTestDatabase database) => _database = database;

    [Fact]
    public async Task CreateAsync_WithPublishAndExpirationDates_PersistsAndReturnsCorrectDates()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();

        var authorId = await _database.GetUserIdByUsernameAsync("user");
        var categoryId = await _database.GetCategoryIdByTitleAsync("Architecture");
        var repository = CreateRepository();

        var publishDate = new DateTimeOffset(2030, 6, 1, 9, 0, 0, TimeSpan.Zero);
        var expirationDate = new DateTimeOffset(2030, 12, 31, 23, 59, 59, TimeSpan.Zero);

        var post = BlogPost.Create(
            authorUserId: authorId,
            categoryId: categoryId,
            title: "Scheduled Post With Expiry",
            summary: null,
            content: "Content",
            publishDate: publishDate,
            expirationDate: expirationDate);

        var saved = await repository.CreateAsync(post);

        Assert.NotNull(saved.PublishDate);
        Assert.NotNull(saved.ExpirationDate);
        Assert.Equal(publishDate.ToUniversalTime(), saved.PublishDate!.Value.ToUniversalTime());
        Assert.Equal(expirationDate.ToUniversalTime(), saved.ExpirationDate!.Value.ToUniversalTime());
    }

    [Fact]
    public async Task CreateAsync_WithNullDates_PersistsAndReturnsNullDates()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();

        var authorId = await _database.GetUserIdByUsernameAsync("user");
        var categoryId = await _database.GetCategoryIdByTitleAsync("Architecture");
        var repository = CreateRepository();

        var post = BlogPost.Create(
            authorUserId: authorId,
            categoryId: categoryId,
            title: "Post Without Dates",
            summary: null,
            content: "Content");

        var saved = await repository.CreateAsync(post);

        Assert.Null(saved.PublishDate);
        Assert.Null(saved.ExpirationDate);
    }

    [Fact]
    public async Task UpdateAsync_WithNewDates_PersistsAndReturnsUpdatedDates()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();

        var authorId = await _database.GetUserIdByUsernameAsync("user");
        var categoryId = await _database.GetCategoryIdByTitleAsync("Architecture");
        var repository = CreateRepository();

        var original = await repository.CreateAsync(BlogPost.Create(
            authorUserId: authorId,
            categoryId: categoryId,
            title: "Post To Update Dates",
            summary: null,
            content: "Content"));

        var newPublish = new DateTimeOffset(2031, 1, 1, 0, 0, 0, TimeSpan.Zero);
        var newExpiration = new DateTimeOffset(2031, 6, 1, 0, 0, 0, TimeSpan.Zero);

        var updated = original.Update(
            title: original.Title,
            summary: original.Summary,
            content: original.Content,
            publishDate: newPublish,
            expirationDate: newExpiration);

        var saved = await repository.UpdateAsync(updated);

        Assert.NotNull(saved.PublishDate);
        Assert.NotNull(saved.ExpirationDate);
        Assert.Equal(newPublish.ToUniversalTime(), saved.PublishDate!.Value.ToUniversalTime());
        Assert.Equal(newExpiration.ToUniversalTime(), saved.ExpirationDate!.Value.ToUniversalTime());
    }

    [Fact]
    public async Task UpdateAsync_ClearingDatesToNull_PersistsNullDates()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();

        var authorId = await _database.GetUserIdByUsernameAsync("user");
        var categoryId = await _database.GetCategoryIdByTitleAsync("Architecture");
        var repository = CreateRepository();

        var publishDate = new DateTimeOffset(2030, 3, 1, 0, 0, 0, TimeSpan.Zero);

        var original = await repository.CreateAsync(BlogPost.Create(
            authorUserId: authorId,
            categoryId: categoryId,
            title: "Post To Clear Dates",
            summary: null,
            content: "Content",
            publishDate: publishDate));

        var cleared = original.Update(
            title: original.Title,
            summary: original.Summary,
            content: original.Content,
            publishDate: null,
            expirationDate: null);

        var saved = await repository.UpdateAsync(cleared);

        Assert.Null(saved.PublishDate);
        Assert.Null(saved.ExpirationDate);
    }

    private PostgreSqlPostRepository CreateRepository() =>
        new(new NpgsqlConnectionFactory(_database.CreateSettings()));
}
