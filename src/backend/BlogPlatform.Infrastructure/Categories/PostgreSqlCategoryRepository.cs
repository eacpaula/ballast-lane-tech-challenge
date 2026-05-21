using BlogPlatform.Application.Abstractions;
using BlogPlatform.Domain.Categories;
using BlogPlatform.Infrastructure.Data;
using Npgsql;

namespace BlogPlatform.Infrastructure.Categories;

public sealed class PostgreSqlCategoryRepository : ICategoryRepository
{
    private readonly NpgsqlConnectionFactory _connectionFactory;

    public PostgreSqlCategoryRepository(NpgsqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<bool> ExistsAndAvailableAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT EXISTS (
                SELECT 1 FROM post_categories
                WHERE id = @id AND available = TRUE
            );
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("id", categoryId);
        var result = await command.ExecuteScalarAsync(cancellationToken);
        return result is true;
    }

    public async Task<bool> TitleExistsAsync(string title, int? excludingCategoryId = null, CancellationToken cancellationToken = default)
    {
        var sql = excludingCategoryId.HasValue
            ? """
                SELECT EXISTS (
                    SELECT 1 FROM post_categories
                    WHERE LOWER(title) = LOWER(@title)
                      AND id <> @excluding_category_id
                );
                """
            : """
                SELECT EXISTS (
                    SELECT 1 FROM post_categories
                    WHERE LOWER(title) = LOWER(@title)
                );
                """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("title", title?.Trim() ?? string.Empty);
        if (excludingCategoryId.HasValue)
        {
            command.Parameters.AddWithValue("excluding_category_id", excludingCategoryId.Value);
        }

        var result = await command.ExecuteScalarAsync(cancellationToken);
        return result is true;
    }

    public async Task<PostCategory> CreateAsync(PostCategory category, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(category);
        const string sql = """
            INSERT INTO post_categories (title, available)
            VALUES (@title, @available)
            RETURNING id, title, available;
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = BuildCategoryCommand(sql, connection, category);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        await reader.ReadAsync(cancellationToken);
        return MapCategory(reader);
    }

    public async Task<PostCategory?> GetByIdAsync(int categoryId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT id, title, available
            FROM post_categories
            WHERE id = @id
            LIMIT 1;
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("id", categoryId);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return MapCategory(reader);
    }

    public async Task<PostCategory> UpdateAsync(PostCategory category, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(category);
        const string sql = """
            UPDATE post_categories
            SET title = @title,
                available = @available,
                update_date = NOW()
            WHERE id = @id
            RETURNING id, title, available;
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = BuildCategoryCommand(sql, connection, category);
        command.Parameters.AddWithValue("id", category.Id);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        await reader.ReadAsync(cancellationToken);
        return MapCategory(reader);
    }

    public async Task<PostCategory> DeactivateAsync(PostCategory category, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(category);
        const string sql = """
            UPDATE post_categories
            SET available = FALSE,
                update_date = NOW()
            WHERE id = @id
            RETURNING id, title, available;
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("id", category.Id);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        await reader.ReadAsync(cancellationToken);
        return MapCategory(reader);
    }

    private static NpgsqlCommand BuildCategoryCommand(string sql, NpgsqlConnection connection, PostCategory category)
    {
        var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("title", category.Title);
        command.Parameters.AddWithValue("available", category.IsAvailable);
        return command;
    }

    private static PostCategory MapCategory(NpgsqlDataReader reader)
    {
        return PostCategory.Rehydrate(reader.GetInt32(0), reader.GetString(1), reader.GetBoolean(2));
    }
}
