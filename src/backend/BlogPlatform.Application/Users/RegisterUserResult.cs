using BlogPlatform.Domain.Users;

namespace BlogPlatform.Application.Users;

public sealed class RegisterUserResult
{
    private RegisterUserResult(
        bool isSuccess,
        int? userId,
        string? nameOrUsername,
        string? email,
        string? errorCode,
        string? errorMessage)
    {
        IsSuccess = isSuccess;
        UserId = userId;
        NameOrUsername = nameOrUsername;
        Email = email;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public bool IsSuccess { get; }

    public int? UserId { get; }

    public string? NameOrUsername { get; }

    public string? Email { get; }

    public string? ErrorCode { get; }

    public string? ErrorMessage { get; }

    public static RegisterUserResult Success(UserAccount user)
    {
        return new RegisterUserResult(
            isSuccess: true,
            userId: user.Id,
            nameOrUsername: user.NameOrUsername,
            email: user.Email,
            errorCode: null,
            errorMessage: null);
    }

    public static RegisterUserResult Failure(string errorCode, string errorMessage)
    {
        return new RegisterUserResult(
            isSuccess: false,
            userId: null,
            nameOrUsername: null,
            email: null,
            errorCode: errorCode,
            errorMessage: errorMessage);
    }
}
