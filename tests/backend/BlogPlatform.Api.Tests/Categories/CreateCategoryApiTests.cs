using System.Net;
using System.Net.Http.Json;
using BlogPlatform.Api.Contracts.Categories;
using BlogPlatform.Api.Tests.TestSupport;
using Microsoft.AspNetCore.Mvc;

namespace BlogPlatform.Api.Tests.Categories;

public sealed class CreateCategoryApiTests : IClassFixture<BlogPlatformApiFactory>
{
    private readonly BlogPlatformApiFactory _factory;

    public CreateCategoryApiTests(BlogPlatformApiFactory factory) => _factory = factory;

    [Fact]
    public async Task CreateCategory_ReturnsCreatedForAdministrator()
    {
        await _factory.Database.ResetToSeedStateAsync();
        using var client = await ApiAuthenticationTestHelper.CreateAdminClientAsync(_factory);

        var response = await client.PostAsJsonAsync("/api/categories", new CreateCategoryRequest("New API Category"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<CategoryResponse>();
        Assert.NotNull(body);
        Assert.True(body!.Id > 0);
    }

    [Fact]
    public async Task CreateCategory_ReturnsConflictForDuplicateTitle()
    {
        await _factory.Database.ResetToSeedStateAsync();
        using var client = await ApiAuthenticationTestHelper.CreateAdminClientAsync(_factory);

        var response = await client.PostAsJsonAsync("/api/categories", new CreateCategoryRequest("Architecture"));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(await response.Content.ReadFromJsonAsync<ProblemDetails>());
    }
}
