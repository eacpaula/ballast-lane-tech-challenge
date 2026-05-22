using BlogPlatform.Application.Abstractions;
using BlogPlatform.Domain.Posts;
using BlogPlatform.Infrastructure.Data;
using Npgsql;
using NpgsqlTypes;

namespace BlogPlatform.Infrastructure.Posts;

public sealed class PostgreSqlPostRepository : IPostRepository
{
    private readonly NpgsqlConnectionFactory _connectionFactory;

    public PostgreSqlPostRepository(NpgsqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<BlogPost> CreateAsync(BlogPost post, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(post);

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
            RETURNING id, user_id, post_category_id, title, description, content, public_post, available;
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = BuildPostCommand(sql, connection, post);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        await reader.ReadAsync(cancellationToken);
        return MapPost(reader);
    }

    public async Task DeleteAsync(int postId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            DELETE FROM posts
            WHERE id = @id;
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("id", postId);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<BlogPost?> GetByIdAsync(int postId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT id, user_id, post_category_id, title, description, content, public_post, available
            FROM posts
            WHERE id = @id
            LIMIT 1;
            """;

        return await QuerySingleAsync(sql, postId, cancellationToken);
    }

    public async Task<BlogPost?> GetByIdForAuthorAsync(int postId, int authorUserId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT id, user_id, post_category_id, title, description, content, public_post, available
            FROM posts
            WHERE id = @id
              AND user_id = @author_user_id
            LIMIT 1;
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("id", postId);
        command.Parameters.AddWithValue("author_user_id", authorUserId);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return MapPost(reader);
    }

    public async Task<BlogPost?> GetPublicReadByIdAsync(int postId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT id, user_id, post_category_id, title, description, content, public_post, available
            FROM posts
            WHERE id = @id
              AND public_post = TRUE
              AND available = TRUE
            LIMIT 1;
            """;

        return await QuerySingleAsync(sql, postId, cancellationToken);
    }

    public async Task<IReadOnlyList<BlogPost>> ListPublicReadAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT id, user_id, post_category_id, title, description, content, public_post, available
            FROM posts
            WHERE public_post = TRUE
              AND available = TRUE
            ORDER BY id;
            """;

        var posts = new List<BlogPost>();

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            posts.Add(MapPost(reader));
        }

        return posts;
    }

    public async Task<IReadOnlyList<BlogPost>> SearchPublicReadAsync(
        string query,
        int? requestingUserId,
        CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT DISTINCT
                p.id,
                p.user_id,
                p.post_category_id,
                p.title,
                p.description,
                p.content,
                p.public_post,
                p.available
            FROM posts p
            INNER JOIN post_categories pc ON pc.id = p.post_category_id
            LEFT JOIN post_tags pt ON pt.post_id = p.id
            LEFT JOIN tags t ON t.id = pt.tag_id
            WHERE p.available = TRUE
              AND (
                    p.public_post = TRUE
                    OR (@requesting_user_id IS NOT NULL AND p.user_id = @requesting_user_id)
                  )
              AND (
                    p.title ILIKE @pattern
                    OR COALESCE(p.description, '') ILIKE @pattern
                    OR p.content ILIKE @pattern
                    OR pc.title ILIKE @pattern
                    OR COALESCE(t.title, '') ILIKE @pattern
                  )
            ORDER BY p.id;
            """;

        var posts = new List<BlogPost>();

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("pattern", $"%{query}%");
        var userParameter = new NpgsqlParameter<int?>("requesting_user_id", NpgsqlDbType.Integer)
        {
            TypedValue = requestingUserId,
        };
        command.Parameters.Add(userParameter);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            posts.Add(MapPost(reader));
        }

        return posts;
    }

    public async Task<IReadOnlyList<BlogPost>> ListByAuthorAsync(int authorUserId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT id, user_id, post_category_id, title, description, content, public_post, available
            FROM posts
            WHERE user_id = @author_user_id
            ORDER BY id;
            """;

        var posts = new List<BlogPost>();

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("author_user_id", authorUserId);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            posts.Add(MapPost(reader));
        }

        return posts;
    }

    public async Task<BlogPost> UpdateAsync(BlogPost post, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(post);

        const string sql = """
            UPDATE posts
            SET
                title = @title,
                description = @description,
                content = @content,
                public_post = @public_post,
                available = @available,
                update_date = NOW(),
                update_user_id = @update_user_id
            WHERE id = @id
            RETURNING id, user_id, post_category_id, title, description, content, public_post, available;
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = BuildPostCommand(sql, connection, post);
        command.Parameters.AddWithValue("id", post.Id);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
        {
            throw new InvalidOperationException("The requested post was not found.");
        }

        return MapPost(reader);
    }

    private async Task<BlogPost?> QuerySingleAsync(string sql, int postId, CancellationToken cancellationToken)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("id", postId);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return MapPost(reader);
    }

    private static NpgsqlCommand BuildPostCommand(string sql, NpgsqlConnection connection, BlogPost post)
    {
        var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("user_id", post.AuthorUserId);
        command.Parameters.AddWithValue("post_category_id", post.CategoryId);
        command.Parameters.AddWithValue("title", post.Title);
        command.Parameters.AddWithValue("description", (object?)post.Summary ?? DBNull.Value);
        command.Parameters.AddWithValue("content", post.Content);
        command.Parameters.AddWithValue("public_post", post.IsPublic);
        command.Parameters.AddWithValue("available", post.IsAvailable);
        command.Parameters.AddWithValue("creation_user_id", post.AuthorUserId);
        command.Parameters.AddWithValue("update_user_id", post.AuthorUserId);
        return command;
    }

    private static BlogPost MapPost(NpgsqlDataReader reader)
    {
        return BlogPost.Rehydrate(
            id: reader.GetInt32(0),
            authorUserId: reader.GetInt32(1),
            categoryId: reader.GetInt32(2),
            title: reader.GetString(3),
            summary: reader.IsDBNull(4) ? null : reader.GetString(4),
            content: reader.GetString(5),
            isPublic: reader.GetBoolean(6),
            isAvailable: reader.GetBoolean(7));
    }
}
