using BlogPlatform.Application.Abstractions;

namespace BlogPlatform.Application.Posts;

public sealed class GetOwnedPostByIdHandler
{
    private readonly IPostRepository _postRepository;

    public GetOwnedPostByIdHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
    }

    public async Task<GetOwnedPostByIdResult> HandleAsync(
        int authenticatedUserId,
        int postId,
        CancellationToken cancellationToken = default)
    {
        if (authenticatedUserId <= 0)
        {
            return GetOwnedPostByIdResult.Failure("AuthenticationRequired", "Authentication is required.");
        }

        var post = await _postRepository.GetByIdForAuthorAsync(postId, authenticatedUserId, cancellationToken);
        if (post is null)
        {
            return GetOwnedPostByIdResult.Failure("PostNotFound", "The requested post was not found.");
        }

        return GetOwnedPostByIdResult.Success(post);
    }
}
