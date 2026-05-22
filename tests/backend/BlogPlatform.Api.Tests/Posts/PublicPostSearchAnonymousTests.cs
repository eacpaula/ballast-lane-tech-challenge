using System.Net;
using System.Net.Http.Json;
using BlogPlatform.Api.Contracts.Posts;
using BlogPlatform.Api.Tests.TestSupport;

namespace BlogPlatform.Api.Tests.Posts;

public sealed class PublicPostSearchAnonymousTests : IClassFixture<BlogPlatformApiFactory>
{
    private readonly BlogPlatformApiFactory _factory;

    public PublicPostSearchAnonymousTests(BlogPlatformApiFactory factory) => _factory = factory;

    [Fact]
    public async Task SearchPublicPosts_AllowsAnonymousSearchAndReturnsMatches()
    {
        await _factory.Database.ResetToSeedStateAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/posts?q=architecture");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var posts = await response.Content.ReadFromJsonAsync<List<PublicPostSummaryResponse>>();
        Assert.NotNull(posts);
        Assert.Single(posts!);
        Assert.Equal("Building a Lightweight Clean Architecture", posts[0].Title);
    }

    [Fact]
    public async Task SearchPublicPosts_WithNoMatches_ReturnsEmptyArray()
    {
        await _factory.Database.ResetToSeedStateAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/posts?q=does-not-exist");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var posts = await response.Content.ReadFromJsonAsync<List<PublicPostSummaryResponse>>();
        Assert.NotNull(posts);
        Assert.Empty(posts!);
    }
}
