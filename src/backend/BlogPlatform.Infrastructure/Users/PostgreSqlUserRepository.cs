using BlogPlatform.Application.Abstractions;
using BlogPlatform.Domain.Users;
using BlogPlatform.Infrastructure.Data;
using Npgsql;

namespace BlogPlatform.Infrastructure.Users;

public sealed class PostgreSqlUserRepository : IUserRepository
{
    private readonly NpgsqlConnectionFactory _connectionFactory;

    public PostgreSqlUserRepository(NpgsqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<UserAccount> CreateAsync(UserAccount user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);

        const string sql = """
            INSERT INTO users (fullname, email, username, password_hash)
            VALUES (@fullname, @email, @username, @password_hash)
            RETURNING id, fullname, email, password_hash;
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("fullname", user.NameOrUsername);
        command.Parameters.AddWithValue("email", user.Email);
        command.Parameters.AddWithValue("username", user.NameOrUsername);
        command.Parameters.AddWithValue("password_hash", user.PasswordHash);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        await reader.ReadAsync(cancellationToken);

        return UserAccount.Rehydrate(
            id: reader.GetInt32(0),
            nameOrUsername: reader.GetString(1),
            email: reader.GetString(2),
            passwordHash: reader.GetString(3));
    }

    public async Task<UserAccount?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT id, fullname, email, password_hash
            FROM users
            WHERE LOWER(email) = LOWER(@email)
            LIMIT 1;
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("email", email?.Trim() ?? string.Empty);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return UserAccount.Rehydrate(
            id: reader.GetInt32(0),
            nameOrUsername: reader.GetString(1),
            email: reader.GetString(2),
            passwordHash: reader.GetString(3));
    }
}
