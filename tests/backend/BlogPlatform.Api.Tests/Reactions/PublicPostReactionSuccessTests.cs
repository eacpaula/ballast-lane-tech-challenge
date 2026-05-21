using System.Net;
using System.Net.Http.Json;
using BlogPlatform.Api.Contracts.Reactions;
using BlogPlatform.Api.Tests.TestSupport;

namespace BlogPlatform.Api.Tests.Reactions;

public sealed class PublicPostReactionSuccessTests : IClassFixture<BlogPlatformApiFactory>
{
    private readonly BlogPlatformApiFactory _factory;

    public PublicPostReactionSuccessTests(BlogPlatformApiFactory factory) => _factory = factory;

    [Theory]
    [InlineData("like")]
    [InlineData("dislike")]
    public async Task ReactToPost_StoresValidReactionForAnonymousVisitor(string reactionType)
    {
        await _factory.Database.ResetToSeedStateAsync();
        var postId = await _factory.Database.GetPostIdByTitleAsync("Using TDD to Drive Interview-Quality Code");
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync($"/api/posts/{postId}/reactions", new ReactToPostRequest(reactionType, "visitor-api-001"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ReactionResponse>();
        Assert.NotNull(body);
        Assert.Equal(reactionType, body!.ReactionType.ToLowerInvariant());
    }
}
