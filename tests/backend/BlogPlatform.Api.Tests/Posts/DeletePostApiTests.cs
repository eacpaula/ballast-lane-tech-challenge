using System.Net;
using System.Net.Http.Json;
using BlogPlatform.Api.Tests.TestSupport;
using Microsoft.AspNetCore.Mvc;

namespace BlogPlatform.Api.Tests.Posts;

public sealed class DeletePostApiTests : IClassFixture<BlogPlatformApiFactory>
{
    private readonly BlogPlatformApiFactory _factory;

    public DeletePostApiTests(BlogPlatformApiFactory factory) => _factory = factory;

    [Fact]
    public async Task DeletePost_ReturnsOkForOwnedPost()
    {
        await _factory.Database.ResetToSeedStateAsync();
        var postId = await _factory.Database.GetPostIdByTitleAsync("Building a Lightweight Clean Architecture");
        using var client = await ApiAuthenticationTestHelper.CreateUserClientAsync(_factory);

        var response = await client.DeleteAsync($"/api/posts/{postId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeletePost_ReturnsForbiddenForNonOwner()
    {
        await _factory.Database.ResetToSeedStateAsync();
        var postId = await _factory.Database.GetPostIdByTitleAsync("Using TDD to Drive Interview-Quality Code");
        using var client = await ApiAuthenticationTestHelper.CreateUserClientAsync(_factory);

        var response = await client.DeleteAsync($"/api/posts/{postId}");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.NotNull(await response.Content.ReadFromJsonAsync<ProblemDetails>());
    }
}
