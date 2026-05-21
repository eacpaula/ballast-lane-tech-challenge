using BlogPlatform.Application.Abstractions;

namespace BlogPlatform.Application.Posts;

public sealed class GetPublicPostByIdHandler
{
    private readonly IPostRepository _postRepository;

    public GetPublicPostByIdHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
    }

    public async Task<GetPublicPostByIdResult> HandleAsync(
        int postId,
        CancellationToken cancellationToken = default)
    {
        var post = await _postRepository.GetPublicReadByIdAsync(postId, cancellationToken);

        if (post is null || !post.IsPubliclyReadable)
        {
            return GetPublicPostByIdResult.Failure(
                errorCode: "PostNotAvailable",
                errorMessage: "The requested post is not available to the public.");
        }

        return GetPublicPostByIdResult.Success(post);
    }
}
