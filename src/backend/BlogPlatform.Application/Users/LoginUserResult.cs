using BlogPlatform.Domain.Users;

namespace BlogPlatform.Application.Users;

public sealed class LoginUserResult
{
    private LoginUserResult(
        bool isSuccess,
        int? userId,
        string? nameOrUsername,
        string? email,
        string? authenticationPayload,
        string? errorCode,
        string? errorMessage)
    {
        IsSuccess = isSuccess;
        UserId = userId;
        NameOrUsername = nameOrUsername;
        Email = email;
        AuthenticationPayload = authenticationPayload;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }

    public bool IsSuccess { get; }

    public int? UserId { get; }

    public string? NameOrUsername { get; }

    public string? Email { get; }

    public string? AuthenticationPayload { get; }

    public string? ErrorCode { get; }

    public string? ErrorMessage { get; }

    public static LoginUserResult Success(UserAccount user, string authenticationPayload)
    {
        return new LoginUserResult(
            isSuccess: true,
            userId: user.Id,
            nameOrUsername: user.NameOrUsername,
            email: user.Email,
            authenticationPayload: authenticationPayload,
            errorCode: null,
            errorMessage: null);
    }

    public static LoginUserResult Failure(string errorCode, string errorMessage)
    {
        return new LoginUserResult(
            isSuccess: false,
            userId: null,
            nameOrUsername: null,
            email: null,
            authenticationPayload: null,
            errorCode: errorCode,
            errorMessage: errorMessage);
    }
}
