using System.Net;
using System.Net.Http.Json;
using BlogPlatform.Api.Contracts.Posts;
using BlogPlatform.Api.Tests.TestSupport;

namespace BlogPlatform.Api.Tests.Posts;

public sealed class PublicPostListTests : IClassFixture<BlogPlatformApiFactory>
{
    private readonly BlogPlatformApiFactory _factory;

    public PublicPostListTests(BlogPlatformApiFactory factory) => _factory = factory;

    [Fact]
    public async Task ListPublicPosts_AllowsAnonymousAccess()
    {
        await _factory.Database.ResetToSeedStateAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/posts");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var posts = await response.Content.ReadFromJsonAsync<PaginatedPublicPostResponse>();
        Assert.NotNull(posts);
        Assert.True(posts!.Items.Count >= 2);
    }

    [Fact]
    public async Task ListPublicPosts_ExcludesScheduledAndExpiredPosts()
    {
        await _factory.Database.ResetToSeedStateAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/posts");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var posts = await response.Content.ReadFromJsonAsync<PaginatedPublicPostResponse>();
        Assert.NotNull(posts);
        Assert.DoesNotContain(posts!.Items, p => p.Title == "Upcoming: Advanced Repository Testing Patterns");
        Assert.DoesNotContain(posts.Items, p => p.Title == "Archived: Early Design Decisions");
    }
}
