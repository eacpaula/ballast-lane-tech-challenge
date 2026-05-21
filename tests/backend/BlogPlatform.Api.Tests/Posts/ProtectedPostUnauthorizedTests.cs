using System.Net;
using System.Net.Http.Json;
using BlogPlatform.Api.Contracts.Posts;
using BlogPlatform.Api.Tests.TestSupport;

namespace BlogPlatform.Api.Tests.Posts;

public sealed class ProtectedPostUnauthorizedTests : IClassFixture<BlogPlatformApiFactory>
{
    private readonly BlogPlatformApiFactory _factory;

    public ProtectedPostUnauthorizedTests(BlogPlatformApiFactory factory) => _factory = factory;

    [Fact]
    public async Task ProtectedPostEndpoints_RejectAnonymousRequests()
    {
        await _factory.Database.ResetToSeedStateAsync();
        using var client = _factory.CreateClient();

        var createResponse = await client.PostAsJsonAsync("/api/posts", new CreatePostRequest(1, "Unauthorized", "summary", "content"));
        var updateResponse = await client.PutAsJsonAsync("/api/posts/1", new UpdatePostRequest("Unauthorized", "summary", "content"));
        var deleteResponse = await client.DeleteAsync("/api/posts/1");

        Assert.Equal(HttpStatusCode.Unauthorized, createResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, updateResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, deleteResponse.StatusCode);
    }
}
