using BlogPlatform.Application.Abstractions;

namespace BlogPlatform.Application.Posts;

public sealed class ListPublicPostsHandler
{
    private readonly IPostRepository _postRepository;

    public ListPublicPostsHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
    }

    public async Task<IReadOnlyList<PublicPostListItem>> HandleAsync(
        string? query = null,
        int? requestingUserId = null,
        CancellationToken cancellationToken = default)
    {
        var normalizedQuery = string.IsNullOrWhiteSpace(query) ? null : query.Trim();
        var posts = normalizedQuery is null
            ? await _postRepository.ListPublicReadAsync(cancellationToken)
            : await _postRepository.SearchPublicReadAsync(normalizedQuery, requestingUserId, cancellationToken);

        var visiblePosts = normalizedQuery is null || !requestingUserId.HasValue
            ? posts.Where(post => post.IsPubliclyReadable)
            : posts.Where(post => post.IsPubliclyReadable || (post.IsAvailable && post.AuthorUserId == requestingUserId.Value));

        return visiblePosts
            .Select(PublicPostListItem.From)
            .ToArray();
    }
}
