using BlogPlatform.Application.Abstractions;

namespace BlogPlatform.Application.Posts;

public sealed class ListOwnedPostsHandler
{
    private readonly IPostRepository _postRepository;

    public ListOwnedPostsHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
    }

    public async Task<IReadOnlyList<OwnedPostListItem>> HandleAsync(
        int authenticatedUserId,
        CancellationToken cancellationToken = default)
    {
        if (authenticatedUserId <= 0)
        {
            return [];
        }

        var posts = await _postRepository.ListByAuthorAsync(authenticatedUserId, cancellationToken);

        return posts
            .Select(OwnedPostListItem.From)
            .ToArray();
    }
}
