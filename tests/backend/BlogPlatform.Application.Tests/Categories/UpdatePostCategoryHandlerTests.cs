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
            description: "Original category description",
            isAvailable: true);

        var categoryRepository = new CategoryRepositoryStub
        {
            GetByIdHandler = categoryId => categoryId == 42 ? existingCategory : null,
            UpdateHandler = category => category,
        };
        var handler = new UpdatePostCategoryHandler(categoryRepository);

        var command = new UpdatePostCategoryCommand(
            AuthenticatedUserId: 11,
            IsAdministrator: true,
            CategoryId: 42,
            Title: "  Testing  ",
            Description: "  Updated category description  ");

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.CategoryId);
        Assert.Equal("Testing", result.Title);
        Assert.Equal("Updated category description", result.Description);
        Assert.True(result.IsAvailable);
        Assert.NotNull(categoryRepository.LastUpdatedCategory);
        Assert.Equal("Testing", categoryRepository.LastUpdatedCategory!.Title);
        Assert.Equal("Updated category description", categoryRepository.LastUpdatedCategory.Description);
    }

    [Fact]
    public async Task HandleAsync_WithNonAdminActor_ReturnsAuthorizationFailure()
    {
        var existingCategory = PostCategory.Rehydrate(
            id: 42,
            title: "Backend",
            description: "Original category description",
            isAvailable: true);

        var categoryRepository = new CategoryRepositoryStub
        {
            GetByIdHandler = categoryId => categoryId == 42 ? existingCategory : null,
        };
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
        var categoryRepository = new CategoryRepositoryStub();
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
            description: "Original category description",
            isAvailable: true);

        var categoryRepository = new CategoryRepositoryStub
        {
            GetByIdHandler = categoryId => categoryId == 42 ? existingCategory : null,
        };
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
            description: "Original category description",
            isAvailable: true);

        var categoryRepository = new CategoryRepositoryStub
        {
            GetByIdHandler = categoryId => categoryId == 42 ? existingCategory : null,
            TitleExistsHandler = (_, _) => true,
        };
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
}
