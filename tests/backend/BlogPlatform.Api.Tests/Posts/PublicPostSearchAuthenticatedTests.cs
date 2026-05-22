using System.Net;
using System.Net.Http.Json;
using BlogPlatform.Api.Contracts.Posts;
using BlogPlatform.Api.Tests.TestSupport;

namespace BlogPlatform.Api.Tests.Posts;

public sealed class PublicPostSearchAuthenticatedTests : IClassFixture<BlogPlatformApiFactory>
{
    private readonly BlogPlatformApiFactory _factory;

    public PublicPostSearchAuthenticatedTests(BlogPlatformApiFactory factory) => _factory = factory;

    [Fact]
    public async Task SearchPublicPosts_ForAuthenticatedUser_IncludesOwnedPrivateMatches()
    {
        await _factory.Database.ResetToSeedStateAsync();
        var fixtures = await SearchPostTestData.InsertPrivateVisibilityFixturesAsync(_factory.Database);
        using var client = await ApiAuthenticationTestHelper.CreateUserClientAsync(_factory);

        var response = await client.GetAsync($"/api/posts?q={SearchPostTestData.SharedSearchTerm}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var posts = await response.Content.ReadFromJsonAsync<List<PublicPostSummaryResponse>>();
        Assert.NotNull(posts);
        Assert.Contains(posts!, post => post.Id == fixtures.OwnedPrivatePostId);
    }

    [Fact]
    public async Task SearchPublicPosts_ForAuthenticatedUser_ExcludesOtherUsersPrivateMatches()
    {
        await _factory.Database.ResetToSeedStateAsync();
        var fixtures = await SearchPostTestData.InsertPrivateVisibilityFixturesAsync(_factory.Database);
        using var client = await ApiAuthenticationTestHelper.CreateUserClientAsync(_factory);

        var response = await client.GetAsync($"/api/posts?q={SearchPostTestData.SharedSearchTerm}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var posts = await response.Content.ReadFromJsonAsync<List<PublicPostSummaryResponse>>();
        Assert.NotNull(posts);
        Assert.DoesNotContain(posts!, post => post.Id == fixtures.OtherUsersPrivatePostId);
    }
}
