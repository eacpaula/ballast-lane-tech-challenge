namespace BlogPlatform.Application.Posts;

public sealed record EditBlogPostCommand(
    int AuthenticatedUserId,
    int PostId,
    string Title,
    string? Summary,
    string Content,
    IReadOnlyList<string>? Tags = null,
    DateTimeOffset? PublishDate = null,
    DateTimeOffset? ExpirationDate = null);
