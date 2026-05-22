using System.Net;
using System.Net.Http.Json;
using BlogPlatform.Api.Contracts.Categories;
using BlogPlatform.Api.Tests.TestSupport;
using Microsoft.AspNetCore.Mvc;

namespace BlogPlatform.Api.Tests.Categories;

public sealed class UpdateCategoryApiTests : IClassFixture<BlogPlatformApiFactory>
{
    private readonly BlogPlatformApiFactory _factory;

    public UpdateCategoryApiTests(BlogPlatformApiFactory factory) => _factory = factory;

    [Fact]
    public async Task UpdateCategory_ReturnsOkForAdministrator()
    {
        await _factory.Database.ResetToSeedStateAsync();
        var categoryId = await _factory.Database.GetCategoryIdByTitleAsync("Architecture");
        using var client = await ApiAuthenticationTestHelper.CreateAdminClientAsync(_factory);

        var response = await client.PutAsJsonAsync(
            $"/api/categories/{categoryId}",
            new UpdateCategoryRequest("Architecture Updated", "Updated from integration tests"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<CategoryResponse>();
        Assert.NotNull(body);
        Assert.Equal("Updated from integration tests", body!.Description);
    }

    [Fact]
    public async Task UpdateCategory_ReturnsNotFoundForMissingCategory()
    {
        await _factory.Database.ResetToSeedStateAsync();
        using var client = await ApiAuthenticationTestHelper.CreateAdminClientAsync(_factory);

        var response = await client.PutAsJsonAsync("/api/categories/999999", new UpdateCategoryRequest("Missing Category", "Missing"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(await response.Content.ReadFromJsonAsync<ProblemDetails>());
    }
}
