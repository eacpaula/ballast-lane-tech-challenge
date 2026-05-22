using BlogPlatform.Application.Abstractions;

namespace BlogPlatform.Application.Posts;

public sealed class ListAllPostCategoriesHandler
{
    private readonly ICategoryRepository _categoryRepository;

    public ListAllPostCategoriesHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    }

    public async Task<PaginatedCategoryResult<ManagedPostCategoryListItem>> HandleAsync(
        bool isAdministrator,
        int? page = null,
        int? pageSize = null,
        CancellationToken cancellationToken = default)
    {
        if (!isAdministrator)
        {
            var emptyRequest = CategoryPageRequest.Create(page, pageSize);
            return new PaginatedCategoryResult<ManagedPostCategoryListItem>([], emptyRequest.Page, emptyRequest.PageSize, 0);
        }

        var request = CategoryPageRequest.Create(page, pageSize);
        var categories = await _categoryRepository.ListAllAsync(request, cancellationToken);
        return categories.Map(ManagedPostCategoryListItem.From);
    }
}
