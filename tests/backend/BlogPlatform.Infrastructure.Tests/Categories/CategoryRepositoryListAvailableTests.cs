using BlogPlatform.Infrastructure.Categories;
using BlogPlatform.Infrastructure.Configuration;
using BlogPlatform.Infrastructure.Data;
using BlogPlatform.Application.Posts;
using BlogPlatform.Infrastructure.Tests.TestSupport;

namespace BlogPlatform.Infrastructure.Tests.Categories;

[Collection("PostgreSqlIntegration")]
public sealed class CategoryRepositoryListAvailableTests : IClassFixture<PostgreSqlIntegrationTestDatabase>
{
    private readonly PostgreSqlIntegrationTestDatabase _database;

    public CategoryRepositoryListAvailableTests(PostgreSqlIntegrationTestDatabase database) => _database = database;

    [Fact]
    public async Task ListAvailableAsync_ReturnsOnlyAvailableCategories()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();
        await _database.InsertCategoryAsync("Hidden Category", isAvailable: false, description: "Should not appear");

        var repository = new PostgreSqlCategoryRepository(
            new NpgsqlConnectionFactory(_database.CreateSettings()));

        var result = await repository.ListAvailableAsync(CategoryPageRequest.Create(page: 1, pageSize: 10));

        CategoryPaginationTestData.AssertValidPage(result, 1, 10);
        Assert.Contains(result.Items, category => category.Title == "Architecture" && category.Description is not null);
        Assert.Contains(result.Items, category => category.Title == "Testing");
        Assert.DoesNotContain(result.Items, category => category.Title == "Hidden Category");
        Assert.All(result.Items, category => Assert.True(category.IsAvailable));
    }

    [Fact]
    public async Task ListAvailableAsync_ReturnsEmptyItemsForPageBeyondRange()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();

        var repository = new PostgreSqlCategoryRepository(
            new NpgsqlConnectionFactory(_database.CreateSettings()));

        var result = await repository.ListAvailableAsync(CategoryPageRequest.Create(page: 5, pageSize: 10));

        Assert.Empty(result.Items);
        Assert.Equal(5, result.Page);
    }
}
