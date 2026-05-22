namespace BlogPlatform.Application.Posts;

public sealed class CategoryPageRequest
{
    public const int DefaultPage = 1;
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 100;

    private CategoryPageRequest(int page, int pageSize)
    {
        Page = page;
        PageSize = pageSize;
    }

    public int Page { get; }

    public int PageSize { get; }

    public int Offset => (Page - 1) * PageSize;

    public static CategoryPageRequest Create(int? page = null, int? pageSize = null)
    {
        var resolvedPage = page ?? DefaultPage;
        var resolvedPageSize = pageSize ?? DefaultPageSize;

        if (resolvedPage <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(page), "page must be greater than 0.");
        }

        if (resolvedPageSize <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize), "pageSize must be greater than 0.");
        }

        if (resolvedPageSize > MaxPageSize)
        {
            throw new ArgumentOutOfRangeException(
                nameof(pageSize),
                $"pageSize must be less than or equal to {MaxPageSize}.");
        }

        return new CategoryPageRequest(resolvedPage, resolvedPageSize);
    }
}
