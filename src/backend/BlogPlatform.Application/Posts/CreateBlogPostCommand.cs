namespace BlogPlatform.Application.Posts;

public sealed record CreateBlogPostCommand(
    int AuthenticatedUserId,
    int CategoryId,
    string Title,
    string? Summary,
    string Content,
    IReadOnlyList<string>? Tags = null,
    DateTimeOffset? PublishDate = null,
    DateTimeOffset? ExpirationDate = null);
