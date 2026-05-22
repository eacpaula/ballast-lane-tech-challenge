using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Categories;

namespace BlogPlatform.Application.Tests.Categories;

public sealed class ListAllCategoriesHandlerTests
{
    [Fact]
    public async Task HandleAsync_ReturnsAvailableAndUnavailableCategoriesForAdministrator()
    {
        var repository = new CategoryRepositoryStub
        {
            ListAllHandler = request => new PaginatedCategoryResult<PostCategory>(
                [
                    PostCategory.Rehydrate(1, "Architecture", "System design posts", isAvailable: true),
                    PostCategory.Rehydrate(2, "Hidden", "Internal only", isAvailable: false),
                ],
                request.Page,
                request.PageSize,
                totalCount: 12),
        };
        var handler = new ListAllPostCategoriesHandler(repository);

        var result = await handler.HandleAsync(isAdministrator: true, page: 2, pageSize: 2);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal(2, result.Page);
        Assert.Equal(2, result.PageSize);
        Assert.Equal(12, result.TotalCount);
        Assert.Equal(6, result.TotalPages);
        Assert.True(result.HasNextPage);
        Assert.Contains(result.Items, category => category.Title == "Hidden" && category.IsAvailable == false && category.Description == "Internal only");
    }

    [Fact]
    public async Task HandleAsync_ReturnsEmptyForNonAdministrator()
    {
        var repository = new CategoryRepositoryStub();
        var handler = new ListAllPostCategoriesHandler(repository);

        var result = await handler.HandleAsync(isAdministrator: false, page: 1, pageSize: 10);

        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.False(repository.ListAllWasCalled);
    }
}
