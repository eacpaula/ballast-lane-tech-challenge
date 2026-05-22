using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Categories;

namespace BlogPlatform.Application.Tests.Categories;

public class DeactivatePostCategoryHandlerTests
{
    [Fact]
    public async Task HandleAsync_WithAdminAndExistingCategory_DeactivatesCategoryAndReturnsSuccess()
    {
        var existingCategory = PostCategory.Rehydrate(
            id: 52,
            title: "Backend",
            isAvailable: true);

        var categoryRepository = new TrackingCategoryRepository(existingCategory);
        var handler = new DeactivatePostCategoryHandler(categoryRepository);

        var command = new DeactivatePostCategoryCommand(
            AuthenticatedUserId: 11,
            IsAdministrator: true,
            CategoryId: 52);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(52, result.CategoryId);
        Assert.Equal("Backend", result.Title);
        Assert.False(result.IsAvailable);
        Assert.NotNull(categoryRepository.DeactivatedCategory);
        Assert.False(categoryRepository.DeactivatedCategory!.IsAvailable);
    }

    [Fact]
    public async Task HandleAsync_WithNonAdminActor_ReturnsAuthorizationFailure()
    {
        var existingCategory = PostCategory.Rehydrate(
            id: 52,
            title: "Backend",
            isAvailable: true);

        var categoryRepository = new TrackingCategoryRepository(existingCategory);
        var handler = new DeactivatePostCategoryHandler(categoryRepository);

        var command = new DeactivatePostCategoryCommand(
            AuthenticatedUserId: 11,
            IsAdministrator: false,
            CategoryId: 52);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("AuthorizationRequired", result.ErrorCode);
        Assert.False(categoryRepository.DeactivateWasCalled);
    }

    [Fact]
    public async Task HandleAsync_WithMissingCategory_ReturnsNotFoundFailure()
    {
        var categoryRepository = new TrackingCategoryRepository(existingCategory: null);
        var handler = new DeactivatePostCategoryHandler(categoryRepository);

        var command = new DeactivatePostCategoryCommand(
            AuthenticatedUserId: 11,
            IsAdministrator: true,
            CategoryId: 52);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("CategoryNotFound", result.ErrorCode);
        Assert.False(categoryRepository.DeactivateWasCalled);
    }

    private sealed class TrackingCategoryRepository(PostCategory? existingCategory) : ICategoryRepository
    {
        public PostCategory? DeactivatedCategory { get; private set; }

        public bool DeactivateWasCalled { get; private set; }

        public Task<IReadOnlyList<PostCategory>> ListAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<IReadOnlyList<PostCategory>> ListAvailableAsync(CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<PostCategory> CreateAsync(PostCategory category, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<PostCategory> DeactivateAsync(PostCategory category, CancellationToken cancellationToken = default)
        {
            DeactivateWasCalled = true;
            DeactivatedCategory = category;
            return Task.FromResult(category);
        }

        public Task<bool> ExistsAndAvailableAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<PostCategory?> GetByIdAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(existingCategory?.Id == categoryId ? existingCategory : null);
        }

        public Task<bool> TitleExistsAsync(string title, int? excludingCategoryId = null, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<PostCategory> UpdateAsync(PostCategory category, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }
    }
}
