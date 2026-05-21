namespace BlogPlatform.Domain.Users;

public sealed class UserAccount
{
    private UserAccount(
        int id,
        string nameOrUsername,
        string email,
        string passwordHash)
    {
        Id = id;
        NameOrUsername = nameOrUsername;
        Email = email;
        PasswordHash = passwordHash;
    }

    public int Id { get; }

    public string NameOrUsername { get; }

    public string Email { get; }

    public string PasswordHash { get; }

    public static UserAccount CreateNew(
        string nameOrUsername,
        string email,
        string passwordHash)
    {
        var normalizedNameOrUsername = NormalizeRequired(nameOrUsername, nameof(nameOrUsername));
        var normalizedEmail = NormalizeEmail(email);
        var normalizedPasswordHash = NormalizeRequired(passwordHash, nameof(passwordHash));

        return new UserAccount(
            id: 0,
            nameOrUsername: normalizedNameOrUsername,
            email: normalizedEmail,
            passwordHash: normalizedPasswordHash);
    }

    public static UserAccount Rehydrate(
        int id,
        string nameOrUsername,
        string email,
        string passwordHash)
    {
        if (id <= 0)
        {
            throw new ArgumentException("User id is required.", nameof(id));
        }

        var normalizedNameOrUsername = NormalizeRequired(nameOrUsername, nameof(nameOrUsername));
        var normalizedEmail = NormalizeEmail(email);
        var normalizedPasswordHash = NormalizeRequired(passwordHash, nameof(passwordHash));

        return new UserAccount(
            id: id,
            nameOrUsername: normalizedNameOrUsername,
            email: normalizedEmail,
            passwordHash: normalizedPasswordHash);
    }

    private static string NormalizeRequired(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{paramName} is required.", paramName);
        }

        return value.Trim();
    }

    private static string NormalizeEmail(string email)
    {
        var normalizedEmail = NormalizeRequired(email, nameof(email)).ToLowerInvariant();

        try
        {
            _ = new System.Net.Mail.MailAddress(normalizedEmail);
        }
        catch (FormatException)
        {
            throw new ArgumentException("email is invalid.", nameof(email));
        }

        return normalizedEmail;
    }
}
