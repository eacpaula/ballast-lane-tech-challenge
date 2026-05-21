namespace BlogPlatform.Application.Users;

public sealed record RegisterUserCommand(
    string NameOrUsername,
    string Email,
    string Password);
