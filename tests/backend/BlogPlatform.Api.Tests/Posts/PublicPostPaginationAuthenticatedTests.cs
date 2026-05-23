using System.Net;
using System.Net.Http.Json;
using BlogPlatform.Api.Contracts.Posts;
using BlogPlatform.Api.Tests.TestSupport;

namespace BlogPlatform.Api.Tests.Posts;

public sealed class PublicPostPaginationAuthenticatedTests : IClassFixture<BlogPlatformApiFactory>
{
    private readonly BlogPlatformApiFactory _factory;

    public PublicPostPaginationAuthenticatedTests(BlogPlatformApiFactory factory) => _factory = factory;

    [Fact]
    public async Task ListPublicPosts_ForAuthenticatedUser_RemainsPublicOnlyWithoutSearch()
    {
        await _factory.Database.ResetToSeedStateAsync();
        var fixtures = await PaginatedPostSearchTestData.InsertPrivateVisibilityFixturesAsync(_factory.Database);
        using var client = await ApiAuthenticationTestHelper.CreateUserClientAsync(_factory);

        var response = await client.GetAsync("/api/posts?page=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var posts = await response.Content.ReadFromJsonAsync<PaginatedPublicPostResponse>();
        Assert.NotNull(posts);
        Assert.DoesNotContain(posts!.Items, post => post.Id == fixtures.OwnedPrivatePostId);
    }

    [Fact]
    public async Task SearchPublicPosts_ForAuthenticatedUser_IncludesOwnedPrivateMatches()
    {
        await _factory.Database.ResetToSeedStateAsync();
        var fixtures = await PaginatedPostSearchTestData.InsertPrivateVisibilityFixturesAsync(_factory.Database);
        using var client = await ApiAuthenticationTestHelper.CreateUserClientAsync(_factory);

        var response = await client.GetAsync($"/api/posts?q={PaginatedPostSearchTestData.SharedSearchTerm}&page=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var posts = await response.Content.ReadFromJsonAsync<PaginatedPublicPostResponse>();
        Assert.NotNull(posts);
        Assert.Contains(posts!.Items, post => post.Id == fixtures.OwnedPrivatePostId);
    }

    [Fact]
    public async Task SearchPublicPosts_ForAuthenticatedUser_ExcludesOtherUsersPrivateMatches()
    {
        await _factory.Database.ResetToSeedStateAsync();
        var fixtures = await PaginatedPostSearchTestData.InsertPrivateVisibilityFixturesAsync(_factory.Database);
        using var client = await ApiAuthenticationTestHelper.CreateUserClientAsync(_factory);

        var response = await client.GetAsync($"/api/posts?q={PaginatedPostSearchTestData.SharedSearchTerm}&page=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var posts = await response.Content.ReadFromJsonAsync<PaginatedPublicPostResponse>();
        Assert.NotNull(posts);
        Assert.DoesNotContain(posts!.Items, post => post.Id == fixtures.OtherUsersPrivatePostId);
    }
}
