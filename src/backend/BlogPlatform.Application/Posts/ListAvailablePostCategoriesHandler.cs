using BlogPlatform.Application.Abstractions;

namespace BlogPlatform.Application.Posts;

public sealed class ListAvailablePostCategoriesHandler
{
    private readonly ICategoryRepository _categoryRepository;

    public ListAvailablePostCategoriesHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    }

    public async Task<IReadOnlyList<AvailablePostCategoryListItem>> HandleAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.ListAvailableAsync(cancellationToken);

        return categories
            .Where(category => category.IsAvailable)
            .Select(AvailablePostCategoryListItem.From)
            .ToArray();
    }
}
