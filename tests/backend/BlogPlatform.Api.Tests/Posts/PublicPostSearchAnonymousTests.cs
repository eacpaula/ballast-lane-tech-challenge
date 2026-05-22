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
        Assert.NotEmpty(posts!);
        Assert.Contains(posts!, p => p.Title == "Building a Lightweight Clean Architecture");
    }

    [Fact]
    public async Task SearchPublicPosts_AnonymousSearch_ExcludesScheduledPost()
    {
        await _factory.Database.ResetToSeedStateAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/posts?q=Advanced+Repository+Testing");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var posts = await response.Content.ReadFromJsonAsync<List<PublicPostSummaryResponse>>();
        Assert.NotNull(posts);
        Assert.DoesNotContain(posts!, p => p.Title == "Upcoming: Advanced Repository Testing Patterns");
    }

    [Fact]
    public async Task SearchPublicPosts_AnonymousSearch_ExcludesExpiredPost()
    {
        await _factory.Database.ResetToSeedStateAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/posts?q=Archived+Early+Design");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var posts = await response.Content.ReadFromJsonAsync<List<PublicPostSummaryResponse>>();
        Assert.NotNull(posts);
        Assert.DoesNotContain(posts!, p => p.Title == "Archived: Early Design Decisions");
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
