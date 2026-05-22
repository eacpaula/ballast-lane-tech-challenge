using BlogPlatform.Domain.Posts;
using BlogPlatform.Infrastructure.Configuration;
using BlogPlatform.Infrastructure.Data;
using BlogPlatform.Infrastructure.Posts;

namespace BlogPlatform.Infrastructure.Tests.Posts;

[Collection("PostgreSqlIntegration")]
public sealed class PostRepositoryUpdateTagTests : IClassFixture<PostgreSqlIntegrationTestDatabase>
{
    private readonly PostgreSqlIntegrationTestDatabase _database;

    public PostRepositoryUpdateTagTests(PostgreSqlIntegrationTestDatabase database) => _database = database;

    [Fact]
    public async Task UpdateAsync_WithNewTags_ReplacesPreviousTags()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();

        var authorId = await _database.GetUserIdByUsernameAsync("user");
        var categoryId = await _database.GetCategoryIdByTitleAsync("Architecture");
        var repository = CreateRepository();

        var post = BlogPost.Create(authorUserId: authorId, categoryId: categoryId,
            title: "Update Tags Test", summary: null, content: "c",
            tags: new[] { "old-tag" });
        var saved = await repository.CreateAsync(post);

        var updated = saved.Update(saved.Title, saved.Summary, saved.Content,
            tags: new[] { "new-tag-a", "new-tag-b" });
        var result = await repository.UpdateAsync(updated);

        Assert.Equal(2, result.Tags.Count);
        Assert.Contains("new-tag-a", result.Tags);
        Assert.Contains("new-tag-b", result.Tags);
        Assert.DoesNotContain("old-tag", result.Tags);
    }

    [Fact]
    public async Task UpdateAsync_ClearingTags_ReturnsEmptyTagList()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();

        var authorId = await _database.GetUserIdByUsernameAsync("user");
        var categoryId = await _database.GetCategoryIdByTitleAsync("Architecture");
        var repository = CreateRepository();

        var post = BlogPost.Create(authorUserId: authorId, categoryId: categoryId,
            title: "Clear Tags Test", summary: null, content: "c",
            tags: new[] { "to-remove" });
        var saved = await repository.CreateAsync(post);

        var updated = saved.Update(saved.Title, saved.Summary, saved.Content,
            tags: Array.Empty<string>());
        var result = await repository.UpdateAsync(updated);

        Assert.Empty(result.Tags);
    }

    [Fact]
    public async Task UpdateAsync_WithSameTags_RetainsTags()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();

        var authorId = await _database.GetUserIdByUsernameAsync("user");
        var categoryId = await _database.GetCategoryIdByTitleAsync("Architecture");
        var repository = CreateRepository();

        var post = BlogPost.Create(authorUserId: authorId, categoryId: categoryId,
            title: "Same Tags Test", summary: null, content: "c",
            tags: new[] { "keep-me" });
        var saved = await repository.CreateAsync(post);

        var updated = saved.Update(saved.Title, saved.Summary, saved.Content,
            tags: new[] { "keep-me" });
        var result = await repository.UpdateAsync(updated);

        Assert.Single(result.Tags);
        Assert.Equal("keep-me", result.Tags[0]);
    }

    [Fact]
    public async Task GetByIdAsync_AfterUpdate_ReturnsUpdatedTags()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();

        var authorId = await _database.GetUserIdByUsernameAsync("user");
        var categoryId = await _database.GetCategoryIdByTitleAsync("Architecture");
        var repository = CreateRepository();

        var post = BlogPost.Create(authorUserId: authorId, categoryId: categoryId,
            title: "Read After Update Tags", summary: null, content: "c",
            tags: new[] { "initial" });
        var saved = await repository.CreateAsync(post);

        var updated = saved.Update(saved.Title, saved.Summary, saved.Content,
            tags: new[] { "updated-tag" });
        await repository.UpdateAsync(updated);

        var read = await repository.GetByIdAsync(saved.Id);

        Assert.NotNull(read);
        Assert.Single(read!.Tags);
        Assert.Equal("updated-tag", read.Tags[0]);
    }

    private PostgreSqlPostRepository CreateRepository() =>
        new PostgreSqlPostRepository(new NpgsqlConnectionFactory(_database.CreateSettings()));
}
