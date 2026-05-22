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
    public async Task SearchPublicPosts_ForAuthenticatedUser_IncludesOwnScheduledPost()
    {
        await _factory.Database.ResetToSeedStateAsync();
        var authorId = await _factory.Database.GetUserIdByUsernameAsync("user");
        var categoryId = await _factory.Database.GetCategoryIdByTitleAsync("Architecture");
        var scheduledPostId = await _factory.Database.InsertPostWithDatesAsync(
            authorId, categoryId, "My Scheduled Search Result", null, "content",
            publishDate: DateTimeOffset.UtcNow.AddDays(7));
        using var client = await ApiAuthenticationTestHelper.CreateUserClientAsync(_factory);

        var response = await client.GetAsync("/api/posts?q=Scheduled+Search+Result");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var posts = await response.Content.ReadFromJsonAsync<List<PublicPostSummaryResponse>>();
        Assert.NotNull(posts);
        Assert.Contains(posts!, p => p.Id == scheduledPostId);
    }

    [Fact]
    public async Task SearchPublicPosts_ForAuthenticatedUser_ExcludesOtherUsersScheduledPost()
    {
        await _factory.Database.ResetToSeedStateAsync();
        var adminId = await _factory.Database.GetUserIdByUsernameAsync("admin");
        var categoryId = await _factory.Database.GetCategoryIdByTitleAsync("Architecture");
        var scheduledPostId = await _factory.Database.InsertPostWithDatesAsync(
            adminId, categoryId, "Other Users Scheduled Post Result", null, "content",
            publishDate: DateTimeOffset.UtcNow.AddDays(7));
        using var client = await ApiAuthenticationTestHelper.CreateUserClientAsync(_factory);

        var response = await client.GetAsync("/api/posts?q=Other+Users+Scheduled+Post");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var posts = await response.Content.ReadFromJsonAsync<List<PublicPostSummaryResponse>>();
        Assert.NotNull(posts);
        Assert.DoesNotContain(posts!, p => p.Id == scheduledPostId);
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
