using System.Net;
using System.Net.Http.Json;
using BlogPlatform.Api.Contracts.Categories;
using BlogPlatform.Api.Tests.TestSupport;

namespace BlogPlatform.Api.Tests.Categories;

public sealed class AvailableCategoryListApiTests : IClassFixture<BlogPlatformApiFactory>
{
    private readonly BlogPlatformApiFactory _factory;

    public AvailableCategoryListApiTests(BlogPlatformApiFactory factory) => _factory = factory;

    [Fact]
    public async Task ListAvailableCategories_AllowsAnonymousAccess()
    {
        await _factory.Database.ResetToSeedStateAsync();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/categories/available");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var categories = await response.Content.ReadFromJsonAsync<List<AvailableCategoryResponse>>();
        Assert.NotNull(categories);
        Assert.Equal(2, categories!.Count);
    }

    [Fact]
    public async Task ListAvailableCategories_ExcludesUnavailableCategories()
    {
        await _factory.Database.ResetToSeedStateAsync();
        await _factory.Database.InsertCategoryAsync("Hidden Category", isAvailable: false);
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/categories/available");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var categories = await response.Content.ReadFromJsonAsync<List<AvailableCategoryResponse>>();
        Assert.NotNull(categories);
        Assert.DoesNotContain(categories!, category => category.Title == "Hidden Category");
    }
}
