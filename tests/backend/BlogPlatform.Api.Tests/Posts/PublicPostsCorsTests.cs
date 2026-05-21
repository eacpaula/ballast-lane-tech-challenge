using System.Net;
using BlogPlatform.Api.Tests.TestSupport;

namespace BlogPlatform.Api.Tests.Posts;

public sealed class PublicPostsCorsTests : IClassFixture<BlogPlatformApiFactory>
{
    private readonly BlogPlatformApiFactory _factory;

    public PublicPostsCorsTests(BlogPlatformApiFactory factory) => _factory = factory;

    [Fact]
    public async Task PublicPosts_ReturnCorsHeadersForConfiguredLocalFrontendOrigin()
    {
        await _factory.Database.ResetToSeedStateAsync();
        using var client = _factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/posts");
        request.Headers.Add("Origin", "http://localhost:5173");

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.TryGetValues("Access-Control-Allow-Origin", out var values));
        Assert.Contains("http://localhost:5173", values);
    }
}
