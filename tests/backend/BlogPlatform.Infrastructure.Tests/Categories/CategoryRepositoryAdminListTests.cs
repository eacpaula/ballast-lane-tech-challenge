using BlogPlatform.Infrastructure.Categories;
using BlogPlatform.Infrastructure.Data;
using BlogPlatform.Application.Posts;
using BlogPlatform.Infrastructure.Tests.TestSupport;

namespace BlogPlatform.Infrastructure.Tests.Categories;

[Collection("PostgreSqlIntegration")]
public sealed class CategoryRepositoryAdminListTests : IClassFixture<PostgreSqlIntegrationTestDatabase>
{
    private readonly PostgreSqlIntegrationTestDatabase _database;

    public CategoryRepositoryAdminListTests(PostgreSqlIntegrationTestDatabase database) => _database = database;

    [Fact]
    public async Task ListAllAsync_ReturnsAvailableAndUnavailableCategories()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();
        await _database.InsertCategoryAsync("Hidden Category", isAvailable: false, description: "Internal only");

        var repository = new PostgreSqlCategoryRepository(
            new NpgsqlConnectionFactory(_database.CreateSettings()));

        var result = await repository.ListAllAsync(CategoryPageRequest.Create(page: 1, pageSize: 10));

        CategoryPaginationTestData.AssertValidPage(result, 1, 10);
        Assert.Contains(result.Items, category => category.Title == "Architecture" && category.IsAvailable);
        Assert.Contains(result.Items, category => category.Title == "Hidden Category" && !category.IsAvailable && category.Description == "Internal only");
    }

    [Fact]
    public async Task ListAllAsync_RespectsRequestedPageSize()
    {
        await _database.VerifyConnectivityAsync();
        await _database.ResetToSeedStateAsync();
        await _database.InsertCategoryAsync("Extra One", isAvailable: true);
        await _database.InsertCategoryAsync("Extra Two", isAvailable: true);

        var repository = new PostgreSqlCategoryRepository(
            new NpgsqlConnectionFactory(_database.CreateSettings()));

        var result = await repository.ListAllAsync(CategoryPageRequest.Create(page: 1, pageSize: 2));

        Assert.Equal(2, result.Items.Count);
        Assert.True(result.HasNextPage);
    }
}
