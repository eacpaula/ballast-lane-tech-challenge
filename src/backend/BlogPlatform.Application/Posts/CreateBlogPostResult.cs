using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Posts;

public sealed class CreateBlogPostResult
{
    private CreateBlogPostResult(
        bool isSuccess,
        int? postId,
        int? authorUserId,
        int? categoryId,
        string? title,
        string? summary,
        string? content,
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

    public string? ErrorCode { get; }

    public string? ErrorMessage { get; }

    public static CreateBlogPostResult Success(BlogPost post)
    {
        return new CreateBlogPostResult(
            isSuccess: true,
            postId: post.Id,
            authorUserId: post.AuthorUserId,
            categoryId: post.CategoryId,
            title: post.Title,
            summary: post.Summary,
            content: post.Content,
            errorCode: null,
            errorMessage: null);
    }

    public static CreateBlogPostResult Failure(string errorCode, string errorMessage)
    {
        return new CreateBlogPostResult(
            isSuccess: false,
            postId: null,
            authorUserId: null,
            categoryId: null,
            title: null,
            summary: null,
            content: null,
            errorCode: errorCode,
            errorMessage: errorMessage);
    }
}
