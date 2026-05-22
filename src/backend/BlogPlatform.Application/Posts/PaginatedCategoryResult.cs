namespace BlogPlatform.Application.Posts;

public sealed class PaginatedCategoryResult<TItem>
{
    public PaginatedCategoryResult(
        IReadOnlyList<TItem> items,
        int page,
        int pageSize,
        int totalCount)
    {
        ArgumentNullException.ThrowIfNull(items);

        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);
        HasNextPage = page < TotalPages;
    }

    public IReadOnlyList<TItem> Items { get; }

    public int Page { get; }

    public int PageSize { get; }

    public int TotalCount { get; }

    public int TotalPages { get; }

    public bool HasNextPage { get; }

    public PaginatedCategoryResult<TResult> Map<TResult>(Func<TItem, TResult> mapper)
    {
        ArgumentNullException.ThrowIfNull(mapper);

        return new PaginatedCategoryResult<TResult>(
            Items.Select(mapper).ToArray(),
            Page,
            PageSize,
            TotalCount);
    }
}
