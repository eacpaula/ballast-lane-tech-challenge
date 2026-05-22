using BlogPlatform.Infrastructure.Configuration;
using Npgsql;

namespace BlogPlatform.Api.Tests.TestSupport;

public sealed class PostgreSqlApiTestDatabase
{
    private readonly string _connectionString;
    private readonly string _repositoryRoot;

    public PostgreSqlApiTestDatabase(PostgreSqlConnectionSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        _connectionString = settings.BuildConnectionString();
        _repositoryRoot = ResolveRepositoryRoot();
    }

    public async Task VerifyConnectivityAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new NpgsqlCommand("SELECT to_regclass('public.users') IS NOT NULL;", connection);
        var result = await command.ExecuteScalarAsync(cancellationToken);

        if (result is not true)
        {
            throw new InvalidOperationException(
                "The PostgreSQL schema is not initialized. Start the local database with docker compose before running API tests.");
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

        foreach (var seedFile in GetOrderedSeedFiles())
        {
            var sql = await File.ReadAllTextAsync(seedFile, cancellationToken);
            await using var command = new NpgsqlCommand(sql, connection);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }

    public async Task<int> GetUserIdByUsernameAsync(string username, CancellationToken cancellationToken = default)
        => await QueryIntAsync("SELECT id FROM users WHERE username = @value LIMIT 1;", username, cancellationToken);

    public async Task<int> GetCategoryIdByTitleAsync(string title, CancellationToken cancellationToken = default)
        => await QueryIntAsync("SELECT id FROM post_categories WHERE title = @value LIMIT 1;", title, cancellationToken);

    public async Task<int> GetPostIdByTitleAsync(string title, CancellationToken cancellationToken = default)
        => await QueryIntAsync("SELECT id FROM posts WHERE title = @value LIMIT 1;", title, cancellationToken);

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

    public async Task<int> InsertPostWithDatesAsync(
        int authorUserId,
        int categoryId,
        string title,
        string? summary,
        string content,
        DateTimeOffset? publishDate = null,
        DateTimeOffset? expirationDate = null,
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
                expire_date,
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
                @publish_date,
                @expire_date,
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
        command.Parameters.AddWithValue("publish_date", (object?)publishDate ?? DBNull.Value);
        command.Parameters.AddWithValue("expire_date", (object?)expirationDate ?? DBNull.Value);
        command.Parameters.AddWithValue("creation_user_id", authorUserId);
        command.Parameters.AddWithValue("update_user_id", authorUserId);
        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result);
    }

    public async Task<int> InsertCategoryAsync(
        string title,
        bool isAvailable = true,
        string? description = null,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            INSERT INTO post_categories (title, description, available)
            VALUES (@title, @description, @available)
            RETURNING id;
            """;

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("title", title);
        command.Parameters.AddWithValue("description", (object?)description ?? DBNull.Value);
        command.Parameters.AddWithValue("available", isAvailable);
        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result);
    }

    public async Task<(string? ReactionType, int? UserId, string? VisitorIdentifier)> GetLatestReactionAsync(
        int postId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT reaction_type, user_id, visitor_identifier
            FROM post_reactions
            WHERE post_id = @post_id
            ORDER BY id DESC
            LIMIT 1;
            """;

        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("post_id", postId);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
        {
            return (null, null, null);
        }

        return (
            reader.GetString(0),
            reader.IsDBNull(1) ? null : reader.GetInt32(1),
            reader.IsDBNull(2) ? null : reader.GetString(2));
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

    private IEnumerable<string> GetOrderedSeedFiles()
    {
        return Directory
            .GetFiles(Path.Combine(_repositoryRoot, "database", "seeds"), "*.sql")
            .OrderBy(path => path, StringComparer.Ordinal);
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

        throw new InvalidOperationException("Repository root could not be resolved for API tests.");
    }
}
