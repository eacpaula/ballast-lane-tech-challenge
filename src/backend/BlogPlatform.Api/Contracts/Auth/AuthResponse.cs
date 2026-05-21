namespace BlogPlatform.Api.Contracts.Auth;

public sealed record AuthResponse(
    int UserId,
    string NameOrUsername,
    string Email,
    string? AuthenticationPayload);
