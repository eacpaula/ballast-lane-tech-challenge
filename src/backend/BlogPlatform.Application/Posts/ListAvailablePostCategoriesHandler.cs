using BlogPlatform.Application.Abstractions;

namespace BlogPlatform.Application.Posts;

public sealed class ListAvailablePostCategoriesHandler
{
    private readonly ICategoryRepository _categoryRepository;

    public ListAvailablePostCategoriesHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    }

    public async Task<PaginatedCategoryResult<AvailablePostCategoryListItem>> HandleAsync(
        int? page = null,
        int? pageSize = null,
        CancellationToken cancellationToken = default)
    {
        var request = CategoryPageRequest.Create(page, pageSize);
        var categories = await _categoryRepository.ListAvailableAsync(request, cancellationToken);
        return categories.Map(AvailablePostCategoryListItem.From);
    }
}
