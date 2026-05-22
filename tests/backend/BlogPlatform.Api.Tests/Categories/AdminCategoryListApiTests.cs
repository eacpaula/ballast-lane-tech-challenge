using System.Net;
using System.Net.Http.Json;
using BlogPlatform.Api.Contracts.Categories;
using BlogPlatform.Api.Tests.TestSupport;

namespace BlogPlatform.Api.Tests.Categories;

public sealed class AdminCategoryListApiTests : IClassFixture<BlogPlatformApiFactory>
{
    private readonly BlogPlatformApiFactory _factory;

    public AdminCategoryListApiTests(BlogPlatformApiFactory factory) => _factory = factory;

    [Fact]
    public async Task ListAllCategories_ReturnsAvailableAndUnavailableCategoriesForAdministrator()
    {
        await _factory.Database.ResetToSeedStateAsync();
        await _factory.Database.InsertCategoryAsync("Hidden Category", isAvailable: false);
        using var client = await _factory.CreateAuthenticatedClientAsync("admin@blogplatform.local", "Admin123!");

        var response = await client.GetAsync("/api/categories");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var categories = await response.Content.ReadFromJsonAsync<List<AdminCategoryListItemResponse>>();
        Assert.NotNull(categories);
        Assert.Contains(categories!, category => category.Title == "Hidden Category" && category.IsAvailable == false);
    }

    [Fact]
    public async Task ListAllCategories_RejectsNonAdministrator()
    {
        await _factory.Database.ResetToSeedStateAsync();
        using var client = await _factory.CreateAuthenticatedClientAsync("user@blogplatform.local", "User123!");

        var response = await client.GetAsync("/api/categories");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
