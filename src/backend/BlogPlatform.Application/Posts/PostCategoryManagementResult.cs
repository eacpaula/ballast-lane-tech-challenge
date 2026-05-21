using BlogPlatform.Domain.Categories;

namespace BlogPlatform.Application.Posts;

public sealed class PostCategoryManagementResult
{
    private PostCategoryManagementResult(
        bool isSuccess,
        int? categoryId,
        string? title,
        bool? isAvailable,
        string? errorCode,
        string? errorMessage)
    {
        IsSuccess = isSuccess;
        CategoryId = categoryId;
        Title = title;
        IsAvailable = isAvailable;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public bool IsSuccess { get; }

    public int? CategoryId { get; }

    public string? Title { get; }

    public bool? IsAvailable { get; }

    public string? ErrorCode { get; }

    public string? ErrorMessage { get; }

    public static PostCategoryManagementResult Success(PostCategory category)
    {
        return new PostCategoryManagementResult(
            isSuccess: true,
            categoryId: category.Id,
            title: category.Title,
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
            isAvailable: null,
            errorCode: errorCode,
            errorMessage: errorMessage);
    }
}
