using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Categories;

namespace BlogPlatform.Application.Tests.Categories;

public class UpdatePostCategoryHandlerTests
{
    [Fact]
    public async Task HandleAsync_WithAdminAndValidInput_UpdatesCategoryAndReturnsSuccess()
    {
        var existingCategory = PostCategory.Rehydrate(
            id: 42,
            title: "Backend",
            isAvailable: true);

        var categoryRepository = new TrackingCategoryRepository(existingCategory);
        var handler = new UpdatePostCategoryHandler(categoryRepository);

        var command = new UpdatePostCategoryCommand(
            AuthenticatedUserId: 11,
            IsAdministrator: true,
            CategoryId: 42,
            Title: "  Testing  ");

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.CategoryId);
        Assert.Equal("Testing", result.Title);
        Assert.True(result.IsAvailable);
        Assert.NotNull(categoryRepository.UpdatedCategory);
        Assert.Equal("Testing", categoryRepository.UpdatedCategory!.Title);
    }

    [Fact]
    public async Task HandleAsync_WithNonAdminActor_ReturnsAuthorizationFailure()
    {
        var existingCategory = PostCategory.Rehydrate(
            id: 42,
            title: "Backend",
            isAvailable: true);

        var categoryRepository = new TrackingCategoryRepository(existingCategory);
        var handler = new UpdatePostCategoryHandler(categoryRepository);

        var command = new UpdatePostCategoryCommand(
            AuthenticatedUserId: 11,
            IsAdministrator: false,
            CategoryId: 42,
            Title: "Testing");

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("AuthorizationRequired", result.ErrorCode);
        Assert.False(categoryRepository.UpdateWasCalled);
    }

    [Fact]
    public async Task HandleAsync_WithMissingCategory_ReturnsNotFoundFailure()
    {
        var categoryRepository = new TrackingCategoryRepository(existingCategory: null);
        var handler = new UpdatePostCategoryHandler(categoryRepository);

        var command = new UpdatePostCategoryCommand(
            AuthenticatedUserId: 11,
            IsAdministrator: true,
            CategoryId: 42,
            Title: "Testing");

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("CategoryNotFound", result.ErrorCode);
        Assert.False(categoryRepository.UpdateWasCalled);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task HandleAsync_WithInvalidTitle_ReturnsValidationFailure(string title)
    {
        var existingCategory = PostCategory.Rehydrate(
            id: 42,
            title: "Backend",
            isAvailable: true);

        var categoryRepository = new TrackingCategoryRepository(existingCategory);
        var handler = new UpdatePostCategoryHandler(categoryRepository);

        var command = new UpdatePostCategoryCommand(
            AuthenticatedUserId: 11,
            IsAdministrator: true,
            CategoryId: 42,
            Title: title);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("ValidationError", result.ErrorCode);
        Assert.False(categoryRepository.UpdateWasCalled);
    }

    [Fact]
    public async Task HandleAsync_WithDuplicateReplacementTitle_ReturnsDuplicateFailure()
    {
        var existingCategory = PostCategory.Rehydrate(
            id: 42,
            title: "Backend",
            isAvailable: true);

        var categoryRepository = new TrackingCategoryRepository(existingCategory, titleExists: true);
        var handler = new UpdatePostCategoryHandler(categoryRepository);

        var command = new UpdatePostCategoryCommand(
            AuthenticatedUserId: 11,
            IsAdministrator: true,
            CategoryId: 42,
            Title: "Testing");

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("DuplicateCategoryTitle", result.ErrorCode);
        Assert.False(categoryRepository.UpdateWasCalled);
    }

    private sealed class TrackingCategoryRepository(PostCategory? existingCategory, bool titleExists = false) : ICategoryRepository
    {
        public PostCategory? UpdatedCategory { get; private set; }

        public bool UpdateWasCalled { get; private set; }

        public Task<PostCategory> CreateAsync(PostCategory category, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<PostCategory> DeactivateAsync(PostCategory category, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
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
            return Task.FromResult(titleExists);
        }

        public Task<PostCategory> UpdateAsync(PostCategory category, CancellationToken cancellationToken = default)
        {
            UpdateWasCalled = true;
            UpdatedCategory = category;
            return Task.FromResult(category);
        }
    }
}
