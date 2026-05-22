using System.Net;
using System.Net.Http.Json;
using BlogPlatform.Api.Contracts.Posts;
using BlogPlatform.Api.Tests.TestSupport;
using Microsoft.AspNetCore.Mvc;

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

    [Fact]
    public async Task CreatePost_WithPublishAndExpirationDates_ReturnsCreatedWithDates()
    {
        await _factory.Database.ResetToSeedStateAsync();
        var categoryId = await _factory.Database.GetCategoryIdByTitleAsync("Architecture");
        using var client = await ApiAuthenticationTestHelper.CreateUserClientAsync(_factory);
        var publishDate = DateTimeOffset.UtcNow.AddDays(1);
        var expirationDate = DateTimeOffset.UtcNow.AddDays(30);

        var response = await client.PostAsJsonAsync("/api/posts", new CreatePostRequest(
            categoryId, "Scheduled Post", "summary", "content",
            PublishDate: publishDate, ExpirationDate: expirationDate));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<PostMutationResponse>();
        Assert.NotNull(body);
        Assert.NotNull(body!.PublishDate);
        Assert.NotNull(body.ExpirationDate);
    }

    [Fact]
    public async Task CreatePost_WithExpirationBeforePublish_ReturnsBadRequest()
    {
        await _factory.Database.ResetToSeedStateAsync();
        var categoryId = await _factory.Database.GetCategoryIdByTitleAsync("Architecture");
        using var client = await ApiAuthenticationTestHelper.CreateUserClientAsync(_factory);
        var publishDate = DateTimeOffset.UtcNow.AddDays(10);
        var expirationDate = DateTimeOffset.UtcNow.AddDays(5);

        var response = await client.PostAsJsonAsync("/api/posts", new CreatePostRequest(
            categoryId, "Bad Date Post", "summary", "content",
            PublishDate: publishDate, ExpirationDate: expirationDate));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(await response.Content.ReadFromJsonAsync<ProblemDetails>());
    }
}
