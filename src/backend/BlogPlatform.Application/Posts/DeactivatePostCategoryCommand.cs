namespace BlogPlatform.Application.Posts;

public sealed record DeactivatePostCategoryCommand(
    int AuthenticatedUserId,
    bool IsAdministrator,
    int CategoryId);
