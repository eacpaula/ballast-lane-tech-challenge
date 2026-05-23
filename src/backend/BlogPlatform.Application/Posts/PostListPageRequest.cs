namespace BlogPlatform.Application.Posts;

public sealed class PostListPageRequest
{
    public const int DefaultPage = 1;
    public const int DefaultPageSize = 6;
    public const int MaxPageSize = 20;

    private PostListPageRequest(string? query, int page, int pageSize, int? requestingUserId)
    {
        Query = string.IsNullOrWhiteSpace(query) ? null : query.Trim();
        Page = page;
        PageSize = pageSize;
        RequestingUserId = requestingUserId;
    }

    public string? Query { get; }

    public int Page { get; }

    public int PageSize { get; }

    public int? RequestingUserId { get; }

    public int Offset => (Page - 1) * PageSize;

    public static PostListPageRequest Create(
        string? query = null,
        int? page = null,
        int? pageSize = null,
        int? requestingUserId = null)
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
            throw new ArgumentOutOfRangeException(nameof(pageSize), $"pageSize must be less than or equal to {MaxPageSize}.");
        }

        return new PostListPageRequest(query, resolvedPage, resolvedPageSize, requestingUserId);
    }
}
