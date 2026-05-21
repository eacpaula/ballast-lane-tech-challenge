namespace BlogPlatform.Api.Contracts.Auth;

public sealed record RegisterUserRequest(
    string NameOrUsername,
    string Email,
    string Password);
