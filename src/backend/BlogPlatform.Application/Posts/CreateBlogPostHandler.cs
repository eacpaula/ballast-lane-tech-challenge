using BlogPlatform.Application.Abstractions;
using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Posts;

public sealed class CreateBlogPostHandler
{
    private readonly IPostRepository _postRepository;
    private readonly ICategoryRepository _categoryRepository;

    public CreateBlogPostHandler(IPostRepository postRepository, ICategoryRepository categoryRepository)
    {
        _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
        _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
    }

    public async Task<CreateBlogPostResult> HandleAsync(
        CreateBlogPostCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.AuthenticatedUserId <= 0)
        {
            return CreateBlogPostResult.Failure(
                errorCode: "AuthenticationRequired",
                errorMessage: "An authenticated user is required to create a post.");
        }

        var categoryAvailable = await _categoryRepository.ExistsAndAvailableAsync(
            command.CategoryId,
            cancellationToken);

        if (!categoryAvailable)
        {
            return CreateBlogPostResult.Failure(
                errorCode: "CategoryNotFoundOrUnavailable",
                errorMessage: "The selected category does not exist or cannot be used.");
        }

        BlogPost post;

        try
        {
            post = BlogPost.Create(
                authorUserId: command.AuthenticatedUserId,
                categoryId: command.CategoryId,
                title: command.Title,
                summary: command.Summary,
                content: command.Content);
        }
        catch (ArgumentException exception)
        {
            return CreateBlogPostResult.Failure(
                errorCode: "ValidationError",
                errorMessage: exception.Message);
        }

        var savedPost = await _postRepository.CreateAsync(post, cancellationToken);

        return CreateBlogPostResult.Success(savedPost);
    }
}
