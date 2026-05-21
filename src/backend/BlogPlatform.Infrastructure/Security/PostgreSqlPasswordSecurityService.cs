using BlogPlatform.Application.Abstractions;

namespace BlogPlatform.Infrastructure.Security;

public sealed class PostgreSqlPasswordSecurityService : IPasswordSecurityService
{
    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("password is required.", nameof(password));
        }

        return BCrypt.Net.BCrypt.HashPassword(password.Trim());
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(passwordHash))
        {
            return false;
        }

        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
