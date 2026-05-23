using System.Net;
using System.Net.Http.Json;
using BlogPlatform.Api.Contracts.Posts;
using BlogPlatform.Api.Tests.TestSupport;

namespace BlogPlatform.Api.Tests.Posts;

public sealed class PostTagsApiTests : IClassFixture<BlogPlatformApiFactory>
{
    private readonly BlogPlatformApiFactory _factory;

    public PostTagsApiTests(BlogPlatformApiFactory factory) => _factory = factory;

    [Fact]
    public async Task CreatePost_WithTags_ReturnsTagsInResponse()
    {
        await _factory.Database.ResetToSeedStateAsync();
        var categoryId = await _factory.Database.GetCategoryIdByTitleAsync("Architecture");
        using var client = await ApiAuthenticationTestHelper.CreateUserClientAsync(_factory);

        var response = await client.PostAsJsonAsync("/api/posts", new CreatePostRequest(
            categoryId, "Tagged Post", null, "content",
            Tags: new[] { "dotnet", "api" }));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PostMutationResponse>();
        Assert.NotNull(body);
        Assert.Equal(2, body!.Tags.Count);
        Assert.Contains("dotnet", body.Tags);
        Assert.Contains("api", body.Tags);
    }

    [Fact]
    public async Task CreatePost_WithNoTags_ReturnsEmptyTagsInResponse()
    {
        await _factory.Database.ResetToSeedStateAsync();
        var categoryId = await _factory.Database.GetCategoryIdByTitleAsync("Architecture");
        using var client = await ApiAuthenticationTestHelper.CreateUserClientAsync(_factory);

        var response = await client.PostAsJsonAsync("/api/posts", new CreatePostRequest(
            categoryId, "Untagged Post", null, "content"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PostMutationResponse>();
        Assert.NotNull(body);
        Assert.Empty(body!.Tags);
    }

    [Fact]
    public async Task UpdatePost_WithNewTags_ReplacesTagsInResponse()
    {
        await _factory.Database.ResetToSeedStateAsync();
        var categoryId = await _factory.Database.GetCategoryIdByTitleAsync("Architecture");
        using var client = await ApiAuthenticationTestHelper.CreateUserClientAsync(_factory);

        var createResponse = await client.PostAsJsonAsync("/api/posts", new CreatePostRequest(
            categoryId, "Post To Update Tags", null, "content",
            Tags: new[] { "old-tag" }));
        var created = await createResponse.Content.ReadFromJsonAsync<PostMutationResponse>();

        var updateResponse = await client.PutAsJsonAsync($"/api/posts/{created!.Id}", new UpdatePostRequest(
            "Post To Update Tags", null, "content",
            Tags: new[] { "new-tag" }));

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var body = await updateResponse.Content.ReadFromJsonAsync<PostMutationResponse>();
        Assert.NotNull(body);
        Assert.Single(body!.Tags);
        Assert.Equal("new-tag", body.Tags[0]);
    }

    [Fact]
    public async Task PublicPostDetail_IncludesTags()
    {
        await _factory.Database.ResetToSeedStateAsync();
        var categoryId = await _factory.Database.GetCategoryIdByTitleAsync("Architecture");
        using var client = await ApiAuthenticationTestHelper.CreateUserClientAsync(_factory);

        var createResponse = await client.PostAsJsonAsync("/api/posts", new CreatePostRequest(
            categoryId, "Public Tagged Post", null, "content",
            Tags: new[] { "public-tag" }));
        var created = await createResponse.Content.ReadFromJsonAsync<PostMutationResponse>();

        using var publicClient = _factory.CreateClient();
        var detailResponse = await publicClient.GetAsync($"/api/posts/{created!.Id}");

        Assert.Equal(HttpStatusCode.OK, detailResponse.StatusCode);
        var body = await detailResponse.Content.ReadFromJsonAsync<PublicPostDetailResponse>();
        Assert.NotNull(body);
        Assert.Single(body!.Tags);
        Assert.Equal("public-tag", body.Tags[0]);
    }

    [Fact]
    public async Task PublicPostList_IncludesTagsForEachPost()
    {
        await _factory.Database.ResetToSeedStateAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/posts");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var posts = await response.Content.ReadFromJsonAsync<PaginatedPublicPostResponse>();
        Assert.NotNull(posts);
        Assert.All(posts!.Items, p => Assert.NotNull(p.Tags));
    }
}
