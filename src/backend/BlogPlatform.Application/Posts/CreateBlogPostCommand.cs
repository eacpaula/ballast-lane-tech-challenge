namespace BlogPlatform.Application.Posts;

public sealed record CreateBlogPostCommand(
    int AuthenticatedUserId,
    int CategoryId,
    string Title,
    string? Summary,
    string Content);
