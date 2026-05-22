using BlogPlatform.Application.Abstractions;
using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Posts;

public sealed class EditBlogPostHandler
{
    private readonly IPostRepository _postRepository;

    public EditBlogPostHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
    }

    public async Task<EditBlogPostResult> HandleAsync(
        EditBlogPostCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.AuthenticatedUserId <= 0)
        {
            return EditBlogPostResult.Failure(
                errorCode: "AuthenticationRequired",
                errorMessage: "An authenticated user is required to edit a post.");
        }

        var existingPost = await _postRepository.GetByIdAsync(command.PostId, cancellationToken);

        if (existingPost is null)
        {
            return EditBlogPostResult.Failure(
                errorCode: "PostNotFound",
                errorMessage: "The requested post was not found.");
        }

        if (existingPost.AuthorUserId != command.AuthenticatedUserId)
        {
            return EditBlogPostResult.Failure(
                errorCode: "ForbiddenPostEdit",
                errorMessage: "Users can edit only posts they own.");
        }

        var normalizedTags = TagNormalizer.Normalize(command.Tags);

        BlogPost updatedPost;

        try
        {
            updatedPost = existingPost.Update(
                title: command.Title,
                summary: command.Summary,
                content: command.Content,
                tags: normalizedTags);
        }
        catch (ArgumentException exception)
        {
            return EditBlogPostResult.Failure(
                errorCode: "ValidationError",
                errorMessage: exception.Message);
        }

        var savedPost = await _postRepository.UpdateAsync(updatedPost, cancellationToken);

        return EditBlogPostResult.Success(savedPost);
    }
}
