namespace BlogPlatform.Api.Contracts.Auth;

public sealed record LoginUserRequest(
    string Email,
    string Password);
