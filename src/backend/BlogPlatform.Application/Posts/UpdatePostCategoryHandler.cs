using BlogPlatform.Application.Abstractions;
using BlogPlatform.Domain.Categories;

namespace BlogPlatform.Application.Posts;

public sealed class UpdatePostCategoryHandler
{
    private readonly ICategoryRepository _categoryRepository;

    public UpdatePostCategoryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    }

    public async Task<PostCategoryManagementResult> HandleAsync(
        UpdatePostCategoryCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.AuthenticatedUserId <= 0 || !command.IsAdministrator)
        {
            return PostCategoryManagementResult.Failure(
                errorCode: "AuthorizationRequired",
                errorMessage: "An administrator is required to manage categories.");
        }

        var existingCategory = await _categoryRepository.GetByIdAsync(command.CategoryId, cancellationToken);

        if (existingCategory is null)
        {
            return PostCategoryManagementResult.Failure(
                errorCode: "CategoryNotFound",
                errorMessage: "The requested category was not found.");
        }

        PostCategory updatedCategory;

        try
        {
            updatedCategory = existingCategory.UpdateDetails(command.Title, command.Description);
        }
        catch (ArgumentException exception)
        {
            return PostCategoryManagementResult.Failure(
                errorCode: "ValidationError",
                errorMessage: exception.Message);
        }

        var titleExists = await _categoryRepository.TitleExistsAsync(
            updatedCategory.Title,
            excludingCategoryId: existingCategory.Id,
            cancellationToken);

        if (titleExists)
        {
            return PostCategoryManagementResult.Failure(
                errorCode: "DuplicateCategoryTitle",
                errorMessage: "A category with the same title already exists.");
        }

        var savedCategory = await _categoryRepository.UpdateAsync(updatedCategory, cancellationToken);

        return PostCategoryManagementResult.Success(savedCategory);
    }
}
