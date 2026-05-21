using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Posts;

public sealed class RemoveBlogPostResult
{
    private RemoveBlogPostResult(
        bool isSuccess,
        int? postId,
        int? authorUserId,
        string? errorCode,
        string? errorMessage)
    {
        IsSuccess = isSuccess;
        PostId = postId;
        AuthorUserId = authorUserId;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public bool IsSuccess { get; }

    public int? PostId { get; }

    public int? AuthorUserId { get; }

    public string? ErrorCode { get; }

    public string? ErrorMessage { get; }

    public static RemoveBlogPostResult Success(BlogPost post)
    {
        return new RemoveBlogPostResult(
            isSuccess: true,
            postId: post.Id,
            authorUserId: post.AuthorUserId,
            errorCode: null,
            errorMessage: null);
    }

    public static RemoveBlogPostResult Failure(string errorCode, string errorMessage)
    {
        return new RemoveBlogPostResult(
            isSuccess: false,
            postId: null,
            authorUserId: null,
            errorCode: errorCode,
            errorMessage: errorMessage);
    }
}
