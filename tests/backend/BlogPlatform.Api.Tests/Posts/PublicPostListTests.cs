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
        var posts = await response.Content.ReadFromJsonAsync<List<PublicPostSummaryResponse>>();
        Assert.NotNull(posts);
        Assert.Equal(2, posts!.Count);
    }
}
