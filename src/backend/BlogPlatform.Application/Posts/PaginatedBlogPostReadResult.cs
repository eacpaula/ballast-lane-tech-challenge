using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Posts;

public sealed class PaginatedBlogPostReadResult
{
    public PaginatedBlogPostReadResult(IReadOnlyList<BlogPost> items, int totalCount)
    {
        ArgumentNullException.ThrowIfNull(items);

        Items = items;
        TotalCount = totalCount;
    }

    public IReadOnlyList<BlogPost> Items { get; }

    public int TotalCount { get; }
}
