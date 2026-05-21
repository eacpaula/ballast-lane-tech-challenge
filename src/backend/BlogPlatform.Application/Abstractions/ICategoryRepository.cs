using BlogPlatform.Domain.Categories;

namespace BlogPlatform.Application.Abstractions;

public interface ICategoryRepository
{
    Task<bool> ExistsAndAvailableAsync(int categoryId, CancellationToken cancellationToken = default);

    Task<bool> TitleExistsAsync(
        string title,
        int? excludingCategoryId = null,
        CancellationToken cancellationToken = default);

    Task<PostCategory> CreateAsync(PostCategory category, CancellationToken cancellationToken = default);

    Task<PostCategory?> GetByIdAsync(int categoryId, CancellationToken cancellationToken = default);

    Task<PostCategory> UpdateAsync(PostCategory category, CancellationToken cancellationToken = default);

    Task<PostCategory> DeactivateAsync(PostCategory category, CancellationToken cancellationToken = default);
}
