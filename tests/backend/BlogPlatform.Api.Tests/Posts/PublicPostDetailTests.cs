using System.Net;
using System.Net.Http.Json;
using BlogPlatform.Api.Contracts.Posts;
using BlogPlatform.Api.Tests.TestSupport;
using Microsoft.AspNetCore.Mvc;

namespace BlogPlatform.Api.Tests.Posts;

public sealed class PublicPostDetailTests : IClassFixture<BlogPlatformApiFactory>
{
    private readonly BlogPlatformApiFactory _factory;

    public PublicPostDetailTests(BlogPlatformApiFactory factory) => _factory = factory;

    [Fact]
    public async Task GetPublicPost_ReturnsPostForPublicAvailableRecord()
    {
        await _factory.Database.ResetToSeedStateAsync();
        var postId = await _factory.Database.GetPostIdByTitleAsync("Building a Lightweight Clean Architecture");
        using var client = _factory.CreateClient();

        var response = await client.GetAsync($"/api/posts/{postId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PublicPostDetailResponse>();
        Assert.NotNull(body);
        Assert.Equal(postId, body!.Id);
    }

    [Fact]
    public async Task GetPublicPost_ReturnsNotFoundForUnavailableOrMissingRecord()
    {
        await _factory.Database.ResetToSeedStateAsync();
        var authorId = await _factory.Database.GetUserIdByUsernameAsync("user");
        var categoryId = await _factory.Database.GetCategoryIdByTitleAsync("Architecture");
        var hiddenPostId = await _factory.Database.InsertPostAsync(authorId, categoryId, "Hidden API Post", "hidden", "hidden content", isPublic: false);
        using var client = _factory.CreateClient();

        var hiddenResponse = await client.GetAsync($"/api/posts/{hiddenPostId}");
        var missingResponse = await client.GetAsync("/api/posts/999999");

        Assert.Equal(HttpStatusCode.NotFound, hiddenResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, missingResponse.StatusCode);
        Assert.NotNull(await hiddenResponse.Content.ReadFromJsonAsync<ProblemDetails>());
    }
}
