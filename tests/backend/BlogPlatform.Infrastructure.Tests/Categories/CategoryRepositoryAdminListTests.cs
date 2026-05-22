using BlogPlatform.Infrastructure.Categories;
using BlogPlatform.Infrastructure.Data;

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
        await _database.InsertCategoryAsync("Hidden Category", isAvailable: false);

        var repository = new PostgreSqlCategoryRepository(
            new NpgsqlConnectionFactory(_database.CreateSettings()));

        var result = await repository.ListAllAsync();

        Assert.Contains(result, category => category.Title == "Architecture" && category.IsAvailable);
        Assert.Contains(result, category => category.Title == "Hidden Category" && !category.IsAvailable);
    }
}
