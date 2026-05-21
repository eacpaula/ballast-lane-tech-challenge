using BlogPlatform.Application.Abstractions;

namespace BlogPlatform.Application.Posts;

public sealed class RemoveBlogPostHandler
{
    private readonly IPostRepository _postRepository;

    public RemoveBlogPostHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
    }

    public async Task<RemoveBlogPostResult> HandleAsync(
        RemoveBlogPostCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.AuthenticatedUserId <= 0)
        {
            return RemoveBlogPostResult.Failure(
                errorCode: "AuthenticationRequired",
                errorMessage: "An authenticated user is required to remove a post.");
        }

        var existingPost = await _postRepository.GetByIdAsync(command.PostId, cancellationToken);

        if (existingPost is null)
        {
            return RemoveBlogPostResult.Failure(
                errorCode: "PostNotFound",
                errorMessage: "The requested post was not found.");
        }

        if (existingPost.AuthorUserId != command.AuthenticatedUserId)
        {
            return RemoveBlogPostResult.Failure(
                errorCode: "ForbiddenPostRemoval",
                errorMessage: "Users can remove only posts they own.");
        }

        await _postRepository.DeleteAsync(existingPost.Id, cancellationToken);

        return RemoveBlogPostResult.Success(existingPost);
    }
}
