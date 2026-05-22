using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Posts;

public sealed class GetOwnedPostByIdResult
{
    private GetOwnedPostByIdResult(
        bool isSuccess,
        int? postId,
        int? authorUserId,
        int? categoryId,
        string? title,
        string? summary,
        string? content,
        bool? isPublic,
        bool? isAvailable,
        string? errorCode,
        string? errorMessage)
    {
        IsSuccess = isSuccess;
        PostId = postId;
        AuthorUserId = authorUserId;
        CategoryId = categoryId;
        Title = title;
        Summary = summary;
        Content = content;
        IsPublic = isPublic;
        IsAvailable = isAvailable;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public bool IsSuccess { get; }
    public int? PostId { get; }
    public int? AuthorUserId { get; }
    public int? CategoryId { get; }
    public string? Title { get; }
    public string? Summary { get; }
    public string? Content { get; }
    public bool? IsPublic { get; }
    public bool? IsAvailable { get; }
    public string? ErrorCode { get; }
    public string? ErrorMessage { get; }

    public static GetOwnedPostByIdResult Success(BlogPost post)
    {
        ArgumentNullException.ThrowIfNull(post);
        return new GetOwnedPostByIdResult(
            true,
            post.Id,
            post.AuthorUserId,
            post.CategoryId,
            post.Title,
            post.Summary,
            post.Content,
            post.IsPublic,
            post.IsAvailable,
            null,
            null);
    }

    public static GetOwnedPostByIdResult Failure(string errorCode, string errorMessage)
        => new(false, null, null, null, null, null, null, null, null, errorCode, errorMessage);
}
