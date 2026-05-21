using System.Net;
using System.Net.Http.Json;
using BlogPlatform.Api.Contracts.Categories;
using BlogPlatform.Api.Tests.TestSupport;

namespace BlogPlatform.Api.Tests.Categories;

public sealed class CategoryAuthorizationApiTests : IClassFixture<BlogPlatformApiFactory>
{
    private readonly BlogPlatformApiFactory _factory;

    public CategoryAuthorizationApiTests(BlogPlatformApiFactory factory) => _factory = factory;

    [Fact]
    public async Task CategoryEndpoints_RejectUnauthorizedAndNonAdminRequests()
    {
        await _factory.Database.ResetToSeedStateAsync();
        using var anonymousClient = _factory.CreateClient();
        using var userClient = await ApiAuthenticationTestHelper.CreateUserClientAsync(_factory);

        var anonymousResponse = await anonymousClient.PostAsJsonAsync("/api/categories", new CreateCategoryRequest("Unauthorized Category"));
        var nonAdminResponse = await userClient.PostAsJsonAsync("/api/categories", new CreateCategoryRequest("User Category"));

        Assert.Equal(HttpStatusCode.Unauthorized, anonymousResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, nonAdminResponse.StatusCode);
    }
}
