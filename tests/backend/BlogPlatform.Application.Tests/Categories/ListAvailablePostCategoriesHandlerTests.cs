using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Categories;

namespace BlogPlatform.Application.Tests.Categories;

public class ListAvailablePostCategoriesHandlerTests
{
    [Fact]
    public async Task HandleAsync_ReturnsOnlyAvailableCategories()
    {
        var repository = new CategoryRepositoryStub
        {
            ListAvailableHandler = request => new PaginatedCategoryResult<PostCategory>(
                [
                    PostCategory.Rehydrate(1, "Architecture", "System design posts", isAvailable: true),
                    PostCategory.Rehydrate(3, "Testing", "Verification posts", isAvailable: true),
                ],
                request.Page,
                request.PageSize,
                totalCount: 2),
        };
        var handler = new ListAvailablePostCategoriesHandler(repository);

        var result = await handler.HandleAsync(page: 2, pageSize: 5);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal(2, result.Page);
        Assert.Equal(5, result.PageSize);
        Assert.Equal(2, result.TotalCount);
        Assert.False(result.HasNextPage);
        Assert.Collection(
            result.Items,
            item =>
            {
                Assert.Equal(1, item.CategoryId);
                Assert.Equal("Architecture", item.Title);
                Assert.Equal("System design posts", item.Description);
                Assert.True(item.IsAvailable);
            },
            item =>
            {
                Assert.Equal(3, item.CategoryId);
                Assert.Equal("Testing", item.Title);
                Assert.Equal("Verification posts", item.Description);
            });
    }

    [Fact]
    public async Task HandleAsync_UsesRepositoryAbstraction()
    {
        var repository = new CategoryRepositoryStub();
        var handler = new ListAvailablePostCategoriesHandler(repository);

        await handler.HandleAsync();

        Assert.True(repository.ListAvailableWasCalled);
    }
}
