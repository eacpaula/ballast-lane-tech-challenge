using System.Net;
using System.Net.Http.Json;
using BlogPlatform.Api.Contracts.Posts;
using BlogPlatform.Api.Tests.TestSupport;

namespace BlogPlatform.Api.Tests.Posts;

public sealed class OwnedPostDetailApiTests : IClassFixture<BlogPlatformApiFactory>
{
    private readonly BlogPlatformApiFactory _factory;

    public OwnedPostDetailApiTests(BlogPlatformApiFactory factory) => _factory = factory;

    [Fact]
    public async Task GetOwnedPost_ReturnsOwnedUnavailablePost()
    {
        await _factory.Database.ResetToSeedStateAsync();
        var categoryId = await _factory.Database.GetCategoryIdByTitleAsync("Architecture");
        var userId = await _factory.Database.GetUserIdByUsernameAsync("user");
        var hiddenPostId = await _factory.Database.InsertPostAsync(userId, categoryId, "Owned Hidden Draft", "summary", "content", isPublic: false);

        using var client = await _factory.CreateAuthenticatedClientAsync("user@blogplatform.local", "User123!");
        var response = await client.GetAsync($"/api/posts/mine/{hiddenPostId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var post = await response.Content.ReadFromJsonAsync<OwnedPostDetailResponse>();
        Assert.NotNull(post);
        Assert.False(post!.IsPublic);
    }

    [Fact]
    public async Task GetOwnedPost_ReturnsNotFoundForPostOwnedByAnotherUser()
    {
        await _factory.Database.ResetToSeedStateAsync();
        using var client = await _factory.CreateAuthenticatedClientAsync("user@blogplatform.local", "User123!");

        var response = await client.GetAsync("/api/posts/mine/2");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
