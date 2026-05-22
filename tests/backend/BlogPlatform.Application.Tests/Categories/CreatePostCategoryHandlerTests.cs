using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Categories;

namespace BlogPlatform.Application.Tests.Categories;

public class CreatePostCategoryHandlerTests
{
    [Fact]
    public async Task HandleAsync_WithAdminAndValidInput_PersistsCategoryAndReturnsSuccess()
    {
        var categoryRepository = new CategoryRepositoryStub
        {
            CreateHandler = category => PostCategory.Rehydrate(
                id: 401,
                title: category.Title,
                description: category.Description,
                isAvailable: category.IsAvailable),
        };
        var handler = new CreatePostCategoryHandler(categoryRepository);

        var command = new CreatePostCategoryCommand(
            AuthenticatedUserId: 17,
            IsAdministrator: true,
            Title: "  Architecture  ",
            Description: "  Foundational platform topics  ");

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(401, result.CategoryId);
        Assert.Equal("Architecture", result.Title);
        Assert.Equal("Foundational platform topics", result.Description);
        Assert.True(result.IsAvailable);
        Assert.NotNull(categoryRepository.LastCreatedCategory);
        Assert.Equal("Architecture", categoryRepository.LastCreatedCategory!.Title);
        Assert.Equal("Foundational platform topics", categoryRepository.LastCreatedCategory.Description);
    }

    [Fact]
    public async Task HandleAsync_WithNonAdminActor_ReturnsAuthorizationFailure()
    {
        var categoryRepository = new CategoryRepositoryStub();
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
        var categoryRepository = new CategoryRepositoryStub();
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
        var categoryRepository = new CategoryRepositoryStub
        {
            TitleExistsHandler = (_, _) => true,
        };
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
}
