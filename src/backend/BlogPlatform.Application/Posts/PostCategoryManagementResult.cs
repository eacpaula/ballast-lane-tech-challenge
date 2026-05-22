using BlogPlatform.Domain.Categories;

namespace BlogPlatform.Application.Posts;

public sealed class PostCategoryManagementResult
{
    private PostCategoryManagementResult(
        bool isSuccess,
        int? categoryId,
        string? title,
        string? description,
        bool? isAvailable,
        string? errorCode,
        string? errorMessage)
    {
        IsSuccess = isSuccess;
        CategoryId = categoryId;
        Title = title;
        Description = description;
        IsAvailable = isAvailable;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public bool IsSuccess { get; }

    public int? CategoryId { get; }

    public string? Title { get; }

    public string? Description { get; }

    public bool? IsAvailable { get; }

    public string? ErrorCode { get; }

    public string? ErrorMessage { get; }

    public static PostCategoryManagementResult Success(PostCategory category)
    {
        return new PostCategoryManagementResult(
            isSuccess: true,
            categoryId: category.Id,
            title: category.Title,
            description: category.Description,
            isAvailable: category.IsAvailable,
            errorCode: null,
            errorMessage: null);
    }

    public static PostCategoryManagementResult Failure(string errorCode, string errorMessage)
    {
        return new PostCategoryManagementResult(
            isSuccess: false,
            categoryId: null,
            title: null,
            description: null,
            isAvailable: null,
            errorCode: errorCode,
            errorMessage: errorMessage);
    }
}
