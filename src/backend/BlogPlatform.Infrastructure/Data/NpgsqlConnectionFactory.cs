using BlogPlatform.Infrastructure.Configuration;
using Npgsql;

namespace BlogPlatform.Infrastructure.Data;

public sealed class NpgsqlConnectionFactory
{
    private readonly string _connectionString;

    public NpgsqlConnectionFactory(PostgreSqlConnectionSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        _connectionString = settings.BuildConnectionString();
    }

    public async Task<NpgsqlConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
