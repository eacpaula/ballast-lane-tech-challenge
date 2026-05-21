using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Categories;

namespace BlogPlatform.Application.Tests.Categories;

public class CreatePostCategoryHandlerTests
{
    [Fact]
    public async Task HandleAsync_WithAdminAndValidInput_PersistsCategoryAndReturnsSuccess()
    {
        var categoryRepository = new TrackingCategoryRepository();
        var handler = new CreatePostCategoryHandler(categoryRepository);

        var command = new CreatePostCategoryCommand(
            AuthenticatedUserId: 17,
            IsAdministrator: true,
            Title: "  Architecture  ");

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(401, result.CategoryId);
        Assert.Equal("Architecture", result.Title);
        Assert.True(result.IsAvailable);
        Assert.NotNull(categoryRepository.CreatedCategory);
        Assert.Equal("Architecture", categoryRepository.CreatedCategory!.Title);
    }

    [Fact]
    public async Task HandleAsync_WithNonAdminActor_ReturnsAuthorizationFailure()
    {
        var categoryRepository = new TrackingCategoryRepository();
        var handler = new CreatePostCategoryHandler(categoryRepository);

        var command = new CreatePostCategoryCommand(
            AuthenticatedUserId: 17,
            IsAdministrator: false,
            Title: "Architecture");

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("AuthorizationRequired", result.ErrorCode);
        Assert.False(categoryRepository.CreateWasCalled);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task HandleAsync_WithInvalidTitle_ReturnsValidationFailure(string title)
    {
        var categoryRepository = new TrackingCategoryRepository();
        var handler = new CreatePostCategoryHandler(categoryRepository);

        var command = new CreatePostCategoryCommand(
            AuthenticatedUserId: 17,
            IsAdministrator: true,
            Title: title);

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("ValidationError", result.ErrorCode);
        Assert.False(categoryRepository.CreateWasCalled);
    }

    [Fact]
    public async Task HandleAsync_WithDuplicateTitle_ReturnsDuplicateFailure()
    {
        var categoryRepository = new TrackingCategoryRepository(titleExists: true);
        var handler = new CreatePostCategoryHandler(categoryRepository);

        var command = new CreatePostCategoryCommand(
            AuthenticatedUserId: 17,
            IsAdministrator: true,
            Title: "Architecture");

        var result = await handler.HandleAsync(command);

        Assert.False(result.IsSuccess);
        Assert.Equal("DuplicateCategoryTitle", result.ErrorCode);
        Assert.False(categoryRepository.CreateWasCalled);
    }

    private sealed class TrackingCategoryRepository(bool titleExists = false) : ICategoryRepository
    {
        public PostCategory? CreatedCategory { get; private set; }

        public bool CreateWasCalled { get; private set; }

        public Task<PostCategory> CreateAsync(PostCategory category, CancellationToken cancellationToken = default)
        {
            CreateWasCalled = true;
            CreatedCategory = PostCategory.Rehydrate(
                id: 401,
                title: category.Title,
                isAvailable: category.IsAvailable);

            return Task.FromResult(CreatedCategory);
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
            throw new NotSupportedException();
        }

        public Task<bool> TitleExistsAsync(string title, int? excludingCategoryId = null, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(titleExists);
        }

        public Task<PostCategory> UpdateAsync(PostCategory category, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }
    }
}
