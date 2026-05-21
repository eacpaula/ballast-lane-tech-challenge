using System.Net;
using BlogPlatform.Api.Tests.TestSupport;

namespace BlogPlatform.Api.Tests.Categories;

public sealed class DeactivateCategoryApiTests : IClassFixture<BlogPlatformApiFactory>
{
    private readonly BlogPlatformApiFactory _factory;

    public DeactivateCategoryApiTests(BlogPlatformApiFactory factory) => _factory = factory;

    [Fact]
    public async Task DeactivateCategory_ReturnsOkForAdministrator()
    {
        await _factory.Database.ResetToSeedStateAsync();
        var categoryId = await _factory.Database.GetCategoryIdByTitleAsync("Testing");
        using var client = await ApiAuthenticationTestHelper.CreateAdminClientAsync(_factory);

        var response = await client.DeleteAsync($"/api/categories/{categoryId}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
