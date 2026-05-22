using BlogPlatform.Application.Abstractions;

namespace BlogPlatform.Application.Posts;

public sealed class ListAllPostCategoriesHandler
{
    private readonly ICategoryRepository _categoryRepository;

    public ListAllPostCategoriesHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    }

    public async Task<IReadOnlyList<ManagedPostCategoryListItem>> HandleAsync(
        bool isAdministrator,
        CancellationToken cancellationToken = default)
    {
        if (!isAdministrator)
        {
            return [];
        }

        var categories = await _categoryRepository.ListAllAsync(cancellationToken);
        return categories.Select(ManagedPostCategoryListItem.From).ToArray();
    }
}
