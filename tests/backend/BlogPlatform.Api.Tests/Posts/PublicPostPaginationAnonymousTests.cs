using System.Net;
using System.Net.Http.Json;
using BlogPlatform.Api.Contracts.Posts;
using BlogPlatform.Api.Tests.TestSupport;

namespace BlogPlatform.Api.Tests.Posts;

public sealed class PublicPostPaginationAnonymousTests : IClassFixture<BlogPlatformApiFactory>
{
    private readonly BlogPlatformApiFactory _factory;

    public PublicPostPaginationAnonymousTests(BlogPlatformApiFactory factory) => _factory = factory;

    [Fact]
    public async Task ListPublicPosts_AllowsAnonymousAccessAndReturnsPaginatedEnvelope()
    {
        await _factory.Database.ResetToSeedStateAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/posts?page=1&pageSize=1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var posts = await response.Content.ReadFromJsonAsync<PaginatedPublicPostResponse>();
        Assert.NotNull(posts);
        Assert.Single(posts!.Items);
        Assert.Equal(1, posts.Page);
        Assert.Equal(1, posts.PageSize);
        Assert.Equal(3, posts.TotalCount);
        Assert.True(posts.HasNextPage);
    }

    [Fact]
    public async Task SearchPublicPosts_WithNoMatches_ReturnsEmptyPaginatedEnvelope()
    {
        await _factory.Database.ResetToSeedStateAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/posts?q=does-not-exist&page=1&pageSize=6");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var posts = await response.Content.ReadFromJsonAsync<PaginatedPublicPostResponse>();
        Assert.NotNull(posts);
        Assert.Empty(posts!.Items);
        Assert.Equal(0, posts.TotalCount);
        Assert.False(posts.HasNextPage);
    }

    [Fact]
    public async Task ListPublicPosts_WithInvalidPage_ReturnsProblemDetails()
    {
        await _factory.Database.ResetToSeedStateAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/posts?page=0&pageSize=6");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
