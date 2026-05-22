using BlogPlatform.Domain.Posts;
using BlogPlatform.Infrastructure.Configuration;
using BlogPlatform.Infrastructure.Data;
using BlogPlatform.Infrastructure.Posts;

namespace BlogPlatform.Infrastructure.Tests.Posts;

[Collection("PostgreSqlIntegration")]
public sealed class PostRepositoryCreateTagTests : IClassFixture<PostgreSqlIntegrationTestDatabase>
{
    private readonly PostgreSqlIntegrationTestDatabase _database;

    public PostRepositoryCreateTagTests(PostgreSqlIntegrationTestDatabase database) => _database = database;

    [Fact]
    public async Task CreateAsync_WithTags_PersiststTagsWithPost()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();

        var authorId = await _database.GetUserIdByUsernameAsync("user");
        var categoryId = await _database.GetCategoryIdByTitleAsync("Architecture");
        var repository = CreateRepository();

        var post = BlogPost.Create(
            authorUserId: authorId,
            categoryId: categoryId,
            title: "Post With Tags",
            summary: null,
            content: "Content",
            tags: new[] { "dotnet", "clean-architecture" });

        var savedPost = await repository.CreateAsync(post);

        Assert.Equal(2, savedPost.Tags.Count);
        Assert.Contains("dotnet", savedPost.Tags);
        Assert.Contains("clean-architecture", savedPost.Tags);
    }

    [Fact]
    public async Task CreateAsync_WithNoTags_ReturnsEmptyTagsOnPost()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();

        var authorId = await _database.GetUserIdByUsernameAsync("user");
        var categoryId = await _database.GetCategoryIdByTitleAsync("Architecture");
        var repository = CreateRepository();

        var post = BlogPost.Create(
            authorUserId: authorId,
            categoryId: categoryId,
            title: "Post Without Tags",
            summary: null,
            content: "Content");

        var savedPost = await repository.CreateAsync(post);

        Assert.Empty(savedPost.Tags);
    }

    [Fact]
    public async Task CreateAsync_WithDuplicateTagTitlesInDatabase_ReusesExistingTagRow()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();

        var authorId = await _database.GetUserIdByUsernameAsync("user");
        var categoryId = await _database.GetCategoryIdByTitleAsync("Architecture");
        var repository = CreateRepository();

        var post1 = BlogPost.Create(authorUserId: authorId, categoryId: categoryId,
            title: "Post One", summary: null, content: "c", tags: new[] { "shared-tag" });
        await repository.CreateAsync(post1);

        var post2 = BlogPost.Create(authorUserId: authorId, categoryId: categoryId,
            title: "Post Two", summary: null, content: "c", tags: new[] { "shared-tag" });
        var savedPost2 = await repository.CreateAsync(post2);

        Assert.Single(savedPost2.Tags);
        Assert.Equal("shared-tag", savedPost2.Tags[0]);
    }

    [Fact]
    public async Task GetByIdAsync_ForPostWithTags_ReturnsTags()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();

        var authorId = await _database.GetUserIdByUsernameAsync("user");
        var categoryId = await _database.GetCategoryIdByTitleAsync("Architecture");
        var repository = CreateRepository();

        var post = BlogPost.Create(authorUserId: authorId, categoryId: categoryId,
            title: "Post With Tags For Read", summary: null, content: "c",
            tags: new[] { "tdd", "ddd" });
        var savedPost = await repository.CreateAsync(post);

        var readPost = await repository.GetByIdAsync(savedPost.Id);

        Assert.NotNull(readPost);
        Assert.Equal(2, readPost!.Tags.Count);
        Assert.Contains("tdd", readPost.Tags);
        Assert.Contains("ddd", readPost.Tags);
    }

    [Fact]
    public async Task GetByIdAsync_ForPostWithNoTags_ReturnsEmptyTagList()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();

        var authorId = await _database.GetUserIdByUsernameAsync("user");
        var categoryId = await _database.GetCategoryIdByTitleAsync("Architecture");
        var repository = CreateRepository();

        var post = BlogPost.Create(authorUserId: authorId, categoryId: categoryId,
            title: "Post Without Tags For Read", summary: null, content: "c");
        var savedPost = await repository.CreateAsync(post);

        var readPost = await repository.GetByIdAsync(savedPost.Id);

        Assert.NotNull(readPost);
        Assert.Empty(readPost!.Tags);
    }

    [Fact]
    public async Task ListPublicReadAsync_IncludesTagsForEachPost()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();

        var posts = await CreateRepository().ListPublicReadAsync();

        Assert.All(posts, p => Assert.NotNull(p.Tags));
        var taggedPost = posts.FirstOrDefault(p => p.Tags.Count > 0);
        Assert.NotNull(taggedPost);
    }

    private PostgreSqlPostRepository CreateRepository() =>
        new PostgreSqlPostRepository(new NpgsqlConnectionFactory(_database.CreateSettings()));
}
