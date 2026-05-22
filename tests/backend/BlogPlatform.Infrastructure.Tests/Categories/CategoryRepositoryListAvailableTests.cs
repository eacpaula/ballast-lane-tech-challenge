using BlogPlatform.Infrastructure.Categories;
using BlogPlatform.Infrastructure.Configuration;
using BlogPlatform.Infrastructure.Data;

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
        await _database.InsertCategoryAsync("Hidden Category", isAvailable: false);

        var repository = new PostgreSqlCategoryRepository(
            new NpgsqlConnectionFactory(_database.CreateSettings()));

        var result = await repository.ListAvailableAsync();

        Assert.Contains(result, category => category.Title == "Architecture");
        Assert.Contains(result, category => category.Title == "Testing");
        Assert.DoesNotContain(result, category => category.Title == "Hidden Category");
        Assert.All(result, category => Assert.True(category.IsAvailable));
    }
}
