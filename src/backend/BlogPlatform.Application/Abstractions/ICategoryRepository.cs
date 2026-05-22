using BlogPlatform.Domain.Categories;
using BlogPlatform.Application.Posts;

namespace BlogPlatform.Application.Abstractions;

public interface ICategoryRepository
{
    Task<PaginatedCategoryResult<PostCategory>> ListAllAsync(
        CategoryPageRequest request,
        CancellationToken cancellationToken = default);

    Task<PaginatedCategoryResult<PostCategory>> ListAvailableAsync(
        CategoryPageRequest request,
        CancellationToken cancellationToken = default);

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
