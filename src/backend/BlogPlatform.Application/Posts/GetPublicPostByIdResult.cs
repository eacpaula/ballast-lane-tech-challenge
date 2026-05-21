using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Posts;

public sealed class GetPublicPostByIdResult
{
    private GetPublicPostByIdResult(
        bool isSuccess,
        int? postId,
        string? title,
        string? summary,
        string? content,
        string? errorCode,
        string? errorMessage)
    {
        IsSuccess = isSuccess;
        PostId = postId;
        Title = title;
        Summary = summary;
        Content = content;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public bool IsSuccess { get; }

    public int? PostId { get; }

    public string? Title { get; }

    public string? Summary { get; }

    public string? Content { get; }

    public string? ErrorCode { get; }

    public string? ErrorMessage { get; }

    public static GetPublicPostByIdResult Success(BlogPost post)
    {
        return new GetPublicPostByIdResult(
            isSuccess: true,
            postId: post.Id,
            title: post.Title,
            summary: post.Summary,
            content: post.Content,
            errorCode: null,
            errorMessage: null);
    }

    public static GetPublicPostByIdResult Failure(string errorCode, string errorMessage)
    {
        return new GetPublicPostByIdResult(
            isSuccess: false,
            postId: null,
            title: null,
            summary: null,
            content: null,
            errorCode: errorCode,
            errorMessage: errorMessage);
    }
}
