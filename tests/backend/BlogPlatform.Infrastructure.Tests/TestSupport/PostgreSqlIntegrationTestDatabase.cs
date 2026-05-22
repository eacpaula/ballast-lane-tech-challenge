using BlogPlatform.Infrastructure.Configuration;
using Npgsql;

namespace BlogPlatform.Infrastructure.Tests;

public sealed class PostgreSqlIntegrationTestDatabase : IAsyncLifetime
{
    private readonly string _connectionString;
    private readonly string _repositoryRoot;

    public PostgreSqlIntegrationTestDatabase()
    {
        _connectionString = CreateSettings().BuildConnectionString();
        _repositoryRoot = ResolveRepositoryRoot();
    }

    public PostgreSqlConnectionSettings CreateSettings()
    {
        static string GetValue(string name, string fallback)
        {
            var value = Environment.GetEnvironmentVariable(name);
            return !string.IsNullOrWhiteSpace(value) ? value.Trim() : fallback;
        }

        var portValue = GetValue("BLOG_PLATFORM_DB_PORT", "5432");

        return new PostgreSqlConnectionSettings(
            host: GetValue("BLOG_PLATFORM_DB_HOST", "localhost"),
            port: int.TryParse(portValue, out var port) ? port : 5432,
            database: GetValue("BLOG_PLATFORM_DB_NAME", "blogplatform"),
            username: GetValue("BLOG_PLATFORM_DB_USER", "blogplatform"),
            password: GetValue("BLOG_PLATFORM_DB_PASSWORD", "blogplatform"));
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => Task.CompletedTask;

    public async Task VerifyConnectivityAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new NpgsqlCommand("SELECT to_regclass('public.post_categories') IS NOT NULL;", connection);
        var result = await command.ExecuteScalarAsync(cancellationToken);

        if (result is not true)
        {
            throw new InvalidOperationException("The PostgreSQL schema is not initialized. Start the local database with docker compose before running Infrastructure tests.");
        }
    }

    public async Task ResetToSeedStateAsync(CancellationToken cancellationToken = default)
    {
        const string truncateSql = """
            TRUNCATE TABLE
                post_reactions,
                post_tags,
                tags,
                posts,
                user_roles,
                roles,
                post_categories,
                users
            RESTART IDENTITY CASCADE;
            """;

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        await using (var truncate = new NpgsqlCommand(truncateSql, connection))
        {
            await truncate.ExecuteNonQueryAsync(cancellationToken);
        }

        foreach (var seedFile in Directory.GetFiles(Path.Combine(_repositoryRoot, "database", "seeds"), "*.sql").OrderBy(path => path, StringComparer.Ordinal))
        {
            var sql = await File.ReadAllTextAsync(seedFile, cancellationToken);
            await using var command = new NpgsqlCommand(sql, connection);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }

    public async Task<int> InsertCategoryAsync(string title, bool isAvailable, CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO post_categories (
                title,
                available,
                creation_user_id,
                update_user_id)
            VALUES (
                @title,
                @available,
                (SELECT id FROM users WHERE username = 'admin'),
                (SELECT id FROM users WHERE username = 'admin'))
            RETURNING id;
            """;

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("title", title);
        command.Parameters.AddWithValue("available", isAvailable);
        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result);
    }

    public async Task<int> GetUserIdByUsernameAsync(string username, CancellationToken cancellationToken = default)
        => await QueryIntAsync("SELECT id FROM users WHERE username = @value LIMIT 1;", username, cancellationToken);

    public async Task<int> GetCategoryIdByTitleAsync(string title, CancellationToken cancellationToken = default)
        => await QueryIntAsync("SELECT id FROM post_categories WHERE title = @value LIMIT 1;", title, cancellationToken);

    public async Task<int> InsertPostAsync(
        int authorUserId,
        int categoryId,
        string title,
        string? summary,
        string content,
        bool isPublic = true,
        bool isAvailable = true,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO posts (
                user_id,
                post_category_id,
                title,
                description,
                content,
                available,
                public_post,
                publish_date,
                creation_user_id,
                update_user_id)
            VALUES (
                @user_id,
                @post_category_id,
                @title,
                @description,
                @content,
                @available,
                @public_post,
                CASE WHEN @public_post THEN NOW() ELSE NULL END,
                @creation_user_id,
                @update_user_id)
            RETURNING id;
            """;

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("user_id", authorUserId);
        command.Parameters.AddWithValue("post_category_id", categoryId);
        command.Parameters.AddWithValue("title", title);
        command.Parameters.AddWithValue("description", (object?)summary ?? DBNull.Value);
        command.Parameters.AddWithValue("content", content);
        command.Parameters.AddWithValue("available", isAvailable);
        command.Parameters.AddWithValue("public_post", isPublic);
        command.Parameters.AddWithValue("creation_user_id", authorUserId);
        command.Parameters.AddWithValue("update_user_id", authorUserId);
        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result);
    }

    private async Task<int> QueryIntAsync(string sql, string value, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("value", value);
        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result);
    }

    private static string ResolveRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "global.json")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new InvalidOperationException("Repository root could not be resolved for Infrastructure tests.");
    }
}
