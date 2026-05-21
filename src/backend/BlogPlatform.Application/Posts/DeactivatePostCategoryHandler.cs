using BlogPlatform.Application.Abstractions;

namespace BlogPlatform.Application.Posts;

public sealed class DeactivatePostCategoryHandler
{
    private readonly ICategoryRepository _categoryRepository;

    public DeactivatePostCategoryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    }

    public async Task<PostCategoryManagementResult> HandleAsync(
        DeactivatePostCategoryCommand command,
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

        var deactivatedCategory = existingCategory.Deactivate();
        var savedCategory = await _categoryRepository.DeactivateAsync(deactivatedCategory, cancellationToken);

        return PostCategoryManagementResult.Success(savedCategory);
    }
}
