using System.Net;
using System.Net.Http.Json;
using BlogPlatform.Api.Contracts.Posts;
using BlogPlatform.Api.Tests.TestSupport;
using Microsoft.AspNetCore.Mvc;

namespace BlogPlatform.Api.Tests.Posts;

public sealed class UpdatePostApiTests : IClassFixture<BlogPlatformApiFactory>
{
    private readonly BlogPlatformApiFactory _factory;

    public UpdatePostApiTests(BlogPlatformApiFactory factory) => _factory = factory;

    [Fact]
    public async Task UpdatePost_ReturnsOkForOwnedPost()
    {
        await _factory.Database.ResetToSeedStateAsync();
        var postId = await _factory.Database.GetPostIdByTitleAsync("Building a Lightweight Clean Architecture");
        using var client = await ApiAuthenticationTestHelper.CreateUserClientAsync(_factory);

        var response = await client.PutAsJsonAsync($"/api/posts/{postId}", new UpdatePostRequest("Updated API Title", "updated", "updated content"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PostMutationResponse>();
        Assert.NotNull(body);
        Assert.Equal(postId, body!.Id);
    }

    [Fact]
    public async Task UpdatePost_ReturnsForbiddenForNonOwner()
    {
        await _factory.Database.ResetToSeedStateAsync();
        var postId = await _factory.Database.GetPostIdByTitleAsync("Using TDD to Drive Interview-Quality Code");
        using var client = await ApiAuthenticationTestHelper.CreateUserClientAsync(_factory);

        var response = await client.PutAsJsonAsync($"/api/posts/{postId}", new UpdatePostRequest("Forbidden", "summary", "content"));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.NotNull(await response.Content.ReadFromJsonAsync<ProblemDetails>());
    }
}
