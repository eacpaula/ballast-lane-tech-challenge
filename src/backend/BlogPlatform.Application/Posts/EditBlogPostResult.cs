using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Posts;

public sealed class EditBlogPostResult
{
    private EditBlogPostResult(
        bool isSuccess,
        int? postId,
        int? authorUserId,
        string? title,
        string? summary,
        string? content,
        IReadOnlyList<string> tags,
        DateTimeOffset? publishDate,
        DateTimeOffset? expirationDate,
        string? errorCode,
        string? errorMessage)
    {
        IsSuccess = isSuccess;
        PostId = postId;
        AuthorUserId = authorUserId;
        Title = title;
        Summary = summary;
        Content = content;
        Tags = tags;
        PublishDate = publishDate;
        ExpirationDate = expirationDate;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public bool IsSuccess { get; }

    public int? PostId { get; }

    public int? AuthorUserId { get; }

    public string? Title { get; }

    public string? Summary { get; }

    public string? Content { get; }

    public IReadOnlyList<string> Tags { get; }

    public DateTimeOffset? PublishDate { get; }

    public DateTimeOffset? ExpirationDate { get; }

    public string? ErrorCode { get; }

    public string? ErrorMessage { get; }

    public static EditBlogPostResult Success(BlogPost post)
    {
        return new EditBlogPostResult(
            isSuccess: true,
            postId: post.Id,
            authorUserId: post.AuthorUserId,
            title: post.Title,
            summary: post.Summary,
            content: post.Content,
            tags: post.Tags,
            publishDate: post.PublishDate,
            expirationDate: post.ExpirationDate,
            errorCode: null,
            errorMessage: null);
    }

    public static EditBlogPostResult Failure(string errorCode, string errorMessage)
    {
        return new EditBlogPostResult(
            isSuccess: false,
            postId: null,
            authorUserId: null,
            title: null,
            summary: null,
            content: null,
            tags: Array.Empty<string>(),
            publishDate: null,
            expirationDate: null,
            errorCode: errorCode,
            errorMessage: errorMessage);
    }
}
