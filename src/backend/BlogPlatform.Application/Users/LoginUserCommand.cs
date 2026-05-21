namespace BlogPlatform.Application.Users;

public sealed record LoginUserCommand(
    string Email,
    string Password);
