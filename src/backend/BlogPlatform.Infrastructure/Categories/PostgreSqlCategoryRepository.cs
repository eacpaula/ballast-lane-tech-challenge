using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;
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

    public async Task<PaginatedCategoryResult<PostCategory>> ListAllAsync(
        CategoryPageRequest request,
        CancellationToken cancellationToken = default)
    {
        return await ListPageAsync(
            """
            SELECT id, title, description, available
            FROM post_categories
            ORDER BY id
            LIMIT @limit OFFSET @offset;
            """,
            "SELECT COUNT(*) FROM post_categories;",
            request,
            cancellationToken);
    }

    public async Task<PaginatedCategoryResult<PostCategory>> ListAvailableAsync(
        CategoryPageRequest request,
        CancellationToken cancellationToken = default)
    {
        return await ListPageAsync(
            """
            SELECT id, title, description, available
            FROM post_categories
            WHERE available = TRUE
            ORDER BY id
            LIMIT @limit OFFSET @offset;
            """,
            "SELECT COUNT(*) FROM post_categories WHERE available = TRUE;",
            request,
            cancellationToken);
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
            INSERT INTO post_categories (title, description, available)
            VALUES (@title, @description, @available)
            RETURNING id, title, description, available;
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
            SELECT id, title, description, available
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
                description = @description,
                available = @available,
                update_date = NOW()
            WHERE id = @id
            RETURNING id, title, description, available;
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
            RETURNING id, title, description, available;
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
        command.Parameters.AddWithValue("description", (object?)category.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("available", category.IsAvailable);
        return command;
    }

    private static PostCategory MapCategory(NpgsqlDataReader reader)
    {
        return PostCategory.Rehydrate(
            reader.GetInt32(0),
            reader.GetString(1),
            reader.IsDBNull(2) ? null : reader.GetString(2),
            reader.GetBoolean(3));
    }

    private async Task<PaginatedCategoryResult<PostCategory>> ListPageAsync(
        string itemsSql,
        string countSql,
        CategoryPageRequest request,
        CancellationToken cancellationToken)
    {
        var categories = new List<PostCategory>();

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using (var countCommand = new NpgsqlCommand(countSql, connection))
        await using (var countReader = await countCommand.ExecuteReaderAsync(cancellationToken))
        {
            await countReader.ReadAsync(cancellationToken);
            var totalCount = countReader.GetInt32(0);
            await countReader.CloseAsync();

            await using var itemsCommand = new NpgsqlCommand(itemsSql, connection);
            itemsCommand.Parameters.AddWithValue("limit", request.PageSize);
            itemsCommand.Parameters.AddWithValue("offset", request.Offset);
            await using var itemsReader = await itemsCommand.ExecuteReaderAsync(cancellationToken);

            while (await itemsReader.ReadAsync(cancellationToken))
            {
                categories.Add(MapCategory(itemsReader));
            }

            return new PaginatedCategoryResult<PostCategory>(categories, request.Page, request.PageSize, totalCount);
        }
    }
}
