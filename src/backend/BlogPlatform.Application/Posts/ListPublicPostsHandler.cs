using BlogPlatform.Application.Abstractions;

namespace BlogPlatform.Application.Posts;

public sealed class ListPublicPostsHandler
{
    private readonly IPostRepository _postRepository;

    public ListPublicPostsHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
    }

    public async Task<IReadOnlyList<PublicPostListItem>> HandleAsync(CancellationToken cancellationToken = default)
    {
        var posts = await _postRepository.ListPublicReadAsync(cancellationToken);

        return posts
            .Where(post => post.IsPubliclyReadable)
            .Select(PublicPostListItem.From)
            .ToArray();
    }
}
