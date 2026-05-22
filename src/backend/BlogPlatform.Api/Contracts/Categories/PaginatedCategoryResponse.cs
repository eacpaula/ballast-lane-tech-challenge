namespace BlogPlatform.Api.Contracts.Categories;

public sealed record PaginatedCategoryResponse<TItem>(
    IReadOnlyList<TItem> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasNextPage);
