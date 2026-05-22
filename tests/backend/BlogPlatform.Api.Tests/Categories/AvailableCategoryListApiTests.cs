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

        var response = await client.GetAsync("/api/categories/available?page=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var categories = await response.Content.ReadFromJsonAsync<PaginatedCategoryResponse<AvailableCategoryResponse>>();
        CategoryPaginationTestData.AssertValidPage(categories, 1, 10);
        Assert.Equal(2, categories!.Items.Count);
    }

    [Fact]
    public async Task ListAvailableCategories_ExcludesUnavailableCategories()
    {
        await _factory.Database.ResetToSeedStateAsync();
        await _factory.Database.InsertCategoryAsync("Hidden Category", isAvailable: false, description: "Should not appear");
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/categories/available?page=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var categories = await response.Content.ReadFromJsonAsync<PaginatedCategoryResponse<AvailableCategoryResponse>>();
        Assert.NotNull(categories);
        Assert.DoesNotContain(categories!.Items, category => category.Title == "Hidden Category");
    }
}
