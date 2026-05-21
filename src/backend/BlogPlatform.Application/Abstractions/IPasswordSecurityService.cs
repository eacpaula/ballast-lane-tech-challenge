namespace BlogPlatform.Application.Abstractions;

public interface IPasswordSecurityService
{
    string HashPassword(string password);

    bool VerifyPassword(string password, string passwordHash);
}
