using System.Net;
using System.Net.Http.Json;
using BlogPlatform.Api.Contracts.Reactions;
using BlogPlatform.Api.Tests.TestSupport;
using Microsoft.AspNetCore.Mvc;

namespace BlogPlatform.Api.Tests.Reactions;

public sealed class PublicPostReactionFailureTests : IClassFixture<BlogPlatformApiFactory>
{
    private readonly BlogPlatformApiFactory _factory;

    public PublicPostReactionFailureTests(BlogPlatformApiFactory factory) => _factory = factory;

    [Fact]
    public async Task ReactToPost_ReturnsBadRequestForInvalidReactionType()
    {
        await _factory.Database.ResetToSeedStateAsync();
        var postId = await _factory.Database.GetPostIdByTitleAsync("Using TDD to Drive Interview-Quality Code");
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync($"/api/posts/{postId}/reactions", new ReactToPostRequest("love", "visitor-api-002"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(await response.Content.ReadFromJsonAsync<ProblemDetails>());
    }

    [Fact]
    public async Task ReactToPost_ReturnsNotFoundForMissingOrUnavailablePost()
    {
        await _factory.Database.ResetToSeedStateAsync();
        var authorId = await _factory.Database.GetUserIdByUsernameAsync("user");
        var categoryId = await _factory.Database.GetCategoryIdByTitleAsync("Architecture");
        var hiddenPostId = await _factory.Database.InsertPostAsync(authorId, categoryId, "Hidden Reaction Post", "hidden", "hidden content", isPublic: false);
        using var client = _factory.CreateClient();

        var hiddenResponse = await client.PostAsJsonAsync($"/api/posts/{hiddenPostId}/reactions", new ReactToPostRequest("like", "visitor-api-003"));
        var missingResponse = await client.PostAsJsonAsync("/api/posts/999999/reactions", new ReactToPostRequest("like", "visitor-api-004"));

        Assert.Equal(HttpStatusCode.NotFound, hiddenResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, missingResponse.StatusCode);
    }
}
