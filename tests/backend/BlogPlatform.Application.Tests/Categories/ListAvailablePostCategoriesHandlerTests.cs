using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Categories;

namespace BlogPlatform.Application.Tests.Categories;

public class ListAvailablePostCategoriesHandlerTests
{
    [Fact]
    public async Task HandleAsync_ReturnsOnlyAvailableCategories()
    {
        var repository = new TrackingCategoryRepository([
            PostCategory.Rehydrate(1, "Architecture", isAvailable: true),
            PostCategory.Rehydrate(2, "Hidden", isAvailable: false),
            PostCategory.Rehydrate(3, "Testing", isAvailable: true),
        ]);
        var handler = new ListAvailablePostCategoriesHandler(repository);

        var result = await handler.HandleAsync();

        Assert.Equal(2, result.Count);
        Assert.Collection(
            result,
            item =>
            {
                Assert.Equal(1, item.CategoryId);
                Assert.Equal("Architecture", item.Title);
            },
            item =>
            {
                Assert.Equal(3, item.CategoryId);
                Assert.Equal("Testing", item.Title);
            });
    }

    [Fact]
    public async Task HandleAsync_UsesRepositoryAbstraction()
    {
        var repository = new TrackingCategoryRepository([]);
        var handler = new ListAvailablePostCategoriesHandler(repository);

        await handler.HandleAsync();

        Assert.True(repository.ListAvailableWasCalled);
    }

    private sealed class TrackingCategoryRepository(IReadOnlyList<PostCategory> categories) : ICategoryRepository
    {
        public bool ListAvailableWasCalled { get; private set; }

        public Task<IReadOnlyList<PostCategory>> ListAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<IReadOnlyList<PostCategory>> ListAvailableAsync(CancellationToken cancellationToken = default)
        {
            ListAvailableWasCalled = true;
            return Task.FromResult(categories);
        }

        public Task<PostCategory> CreateAsync(PostCategory category, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<PostCategory> DeactivateAsync(PostCategory category, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<bool> ExistsAndAvailableAsync(int categoryId, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<PostCategory?> GetByIdAsync(int categoryId, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<bool> TitleExistsAsync(string title, int? excludingCategoryId = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<PostCategory> UpdateAsync(PostCategory category, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }
}
