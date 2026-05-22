namespace BlogPlatform.Application.Posts;

public sealed record UpdatePostCategoryCommand(
    int AuthenticatedUserId,
    bool IsAdministrator,
    int CategoryId,
    string Title,
    string? Description = null);
