using System.Net;
using System.Net.Http.Json;
using BlogPlatform.Api.Contracts.Posts;
using BlogPlatform.Api.Tests.TestSupport;

namespace BlogPlatform.Api.Tests.Posts;

public sealed class CreatePostApiTests : IClassFixture<BlogPlatformApiFactory>
{
    private readonly BlogPlatformApiFactory _factory;

    public CreatePostApiTests(BlogPlatformApiFactory factory) => _factory = factory;

    [Fact]
    public async Task CreatePost_ReturnsCreatedForAuthenticatedUser()
    {
        await _factory.Database.ResetToSeedStateAsync();
        var categoryId = await _factory.Database.GetCategoryIdByTitleAsync("Architecture");
        using var client = await ApiAuthenticationTestHelper.CreateUserClientAsync(_factory);

        var response = await client.PostAsJsonAsync("/api/posts", new CreatePostRequest(categoryId, "API Created Post", "summary", "content"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PostMutationResponse>();
        Assert.NotNull(body);
        Assert.True(body!.Id > 0);
    }
}
