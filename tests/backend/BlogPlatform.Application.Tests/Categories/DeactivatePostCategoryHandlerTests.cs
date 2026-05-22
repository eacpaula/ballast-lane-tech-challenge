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

        var categoryRepository = new CategoryRepositoryStub
        {
            GetByIdHandler = categoryId => categoryId == 52 ? existingCategory : null,
            DeactivateHandler = category => category,
        };
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
        Assert.NotNull(categoryRepository.LastDeactivatedCategory);
        Assert.False(categoryRepository.LastDeactivatedCategory!.IsAvailable);
    }

    [Fact]
    public async Task HandleAsync_WithNonAdminActor_ReturnsAuthorizationFailure()
    {
        var existingCategory = PostCategory.Rehydrate(
            id: 52,
            title: "Backend",
            isAvailable: true);

        var categoryRepository = new CategoryRepositoryStub
        {
            GetByIdHandler = categoryId => categoryId == 52 ? existingCategory : null,
        };
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
        var categoryRepository = new CategoryRepositoryStub();
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
}
