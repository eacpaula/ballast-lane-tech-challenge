using System.Net;
using System.Net.Http.Json;
using BlogPlatform.Api.Contracts.Posts;
using BlogPlatform.Api.Tests.TestSupport;

namespace BlogPlatform.Api.Tests.Posts;

public sealed class OwnedPostListApiTests : IClassFixture<BlogPlatformApiFactory>
{
    private readonly BlogPlatformApiFactory _factory;

    public OwnedPostListApiTests(BlogPlatformApiFactory factory) => _factory = factory;

    [Fact]
    public async Task ListOwnedPosts_ReturnsOnlyPostsForAuthenticatedAuthor()
    {
        await _factory.Database.ResetToSeedStateAsync();
        using var client = await _factory.CreateAuthenticatedClientAsync("user@blogplatform.local", "User123!");

        var response = await client.GetAsync("/api/posts/mine");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var posts = await response.Content.ReadFromJsonAsync<List<OwnedPostSummaryResponse>>();
        Assert.NotNull(posts);
        Assert.NotEmpty(posts!);
        Assert.All(posts!, p => Assert.True(p.Id > 0));
    }
}
