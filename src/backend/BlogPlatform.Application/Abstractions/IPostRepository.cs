using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Abstractions;

public interface IPostRepository
{
    Task<BlogPost> CreateAsync(BlogPost post, CancellationToken cancellationToken = default);

    Task DeleteAsync(int postId, CancellationToken cancellationToken = default);

    Task<BlogPost?> GetByIdAsync(int postId, CancellationToken cancellationToken = default);

    Task<BlogPost?> GetByIdForAuthorAsync(int postId, int authorUserId, CancellationToken cancellationToken = default);

    Task<BlogPost?> GetPublicReadByIdAsync(int postId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BlogPost>> ListByAuthorAsync(int authorUserId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BlogPost>> ListPublicReadAsync(CancellationToken cancellationToken = default);

    async Task<BlogPlatform.Application.Posts.PaginatedBlogPostReadResult> ListPublicReadPageAsync(
        BlogPlatform.Application.Posts.PostListPageRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var items = request.Query is null
            ? await ListPublicReadAsync(cancellationToken)
            : await SearchPublicReadAsync(request.Query, request.RequestingUserId, cancellationToken);

        return new BlogPlatform.Application.Posts.PaginatedBlogPostReadResult(items, items.Count);
    }

    Task<IReadOnlyList<BlogPost>> SearchPublicReadAsync(
        string query,
        int? requestingUserId,
        CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    Task<BlogPost> UpdateAsync(BlogPost post, CancellationToken cancellationToken = default);
}
