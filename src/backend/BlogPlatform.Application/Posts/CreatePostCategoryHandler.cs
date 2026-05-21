using BlogPlatform.Application.Abstractions;
using BlogPlatform.Domain.Categories;

namespace BlogPlatform.Application.Posts;

public sealed class CreatePostCategoryHandler
{
    private readonly ICategoryRepository _categoryRepository;

    public CreatePostCategoryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    }

    public async Task<PostCategoryManagementResult> HandleAsync(
        CreatePostCategoryCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.AuthenticatedUserId <= 0 || !command.IsAdministrator)
        {
            return PostCategoryManagementResult.Failure(
                errorCode: "AuthorizationRequired",
                errorMessage: "An administrator is required to manage categories.");
        }

        PostCategory category;

        try
        {
            category = PostCategory.CreateNew(command.Title);
        }
        catch (ArgumentException exception)
        {
            return PostCategoryManagementResult.Failure(
                errorCode: "ValidationError",
                errorMessage: exception.Message);
        }

        var titleExists = await _categoryRepository.TitleExistsAsync(
            category.Title,
            excludingCategoryId: null,
            cancellationToken);

        if (titleExists)
        {
            return PostCategoryManagementResult.Failure(
                errorCode: "DuplicateCategoryTitle",
                errorMessage: "A category with the same title already exists.");
        }

        var savedCategory = await _categoryRepository.CreateAsync(category, cancellationToken);

        return PostCategoryManagementResult.Success(savedCategory);
    }
}
