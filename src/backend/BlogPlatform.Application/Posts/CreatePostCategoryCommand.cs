namespace BlogPlatform.Application.Posts;

public sealed record CreatePostCategoryCommand(
    int AuthenticatedUserId,
    bool IsAdministrator,
    string Title);
