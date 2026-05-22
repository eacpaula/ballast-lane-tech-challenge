using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Categories;

namespace BlogPlatform.Application.Tests.Categories;

public sealed class ListAllCategoriesHandlerTests
{
    [Fact]
    public async Task HandleAsync_ReturnsAvailableAndUnavailableCategoriesForAdministrator()
    {
        var repository = new TrackingCategoryRepository([
            PostCategory.Rehydrate(1, "Architecture", isAvailable: true),
            PostCategory.Rehydrate(2, "Hidden", isAvailable: false),
        ]);
        var handler = new ListAllPostCategoriesHandler(repository);

        var result = await handler.HandleAsync(isAdministrator: true);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, category => category.Title == "Hidden" && category.IsAvailable == false);
    }

    [Fact]
    public async Task HandleAsync_ReturnsEmptyForNonAdministrator()
    {
        var repository = new TrackingCategoryRepository([PostCategory.Rehydrate(1, "Architecture")]);
        var handler = new ListAllPostCategoriesHandler(repository);

        var result = await handler.HandleAsync(isAdministrator: false);

        Assert.Empty(result);
        Assert.False(repository.ListAllWasCalled);
    }

    private sealed class TrackingCategoryRepository(IReadOnlyList<PostCategory> categories) : ICategoryRepository
    {
        public bool ListAllWasCalled { get; private set; }

        public Task<IReadOnlyList<PostCategory>> ListAllAsync(CancellationToken cancellationToken = default)
        {
            ListAllWasCalled = true;
            return Task.FromResult(categories);
        }

        public Task<IReadOnlyList<PostCategory>> ListAvailableAsync(CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<bool> ExistsAndAvailableAsync(int categoryId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<bool> TitleExistsAsync(string title, int? excludingCategoryId = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<PostCategory> CreateAsync(PostCategory category, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<PostCategory?> GetByIdAsync(int categoryId, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<PostCategory> UpdateAsync(PostCategory category, CancellationToken cancellationToken = default) => throw new NotSupportedException();
        public Task<PostCategory> DeactivateAsync(PostCategory category, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    }
}
