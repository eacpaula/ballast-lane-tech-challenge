namespace BlogPlatform.Application.Posts;

public sealed record RemoveBlogPostCommand(
    int AuthenticatedUserId,
    int PostId);
