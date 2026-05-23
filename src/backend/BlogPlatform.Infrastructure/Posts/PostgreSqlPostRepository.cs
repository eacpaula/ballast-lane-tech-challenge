using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;
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

        const string insertPostSql = """
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
            RETURNING id, user_id, post_category_id, title, description, content, public_post, available, publish_date, expire_date;
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        int postId;
        BlogPost createdPost;

        await using (var command = BuildPostInsertCommand(insertPostSql, connection, transaction, post))
        {
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            await reader.ReadAsync(cancellationToken);
            postId = reader.GetInt32(0);
            createdPost = MapPostFromReader(reader, post.Tags);
        }

        await PersistTagsAsync(connection, transaction, postId, post.AuthorUserId, post.Tags, cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return createdPost;
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
            SELECT
                p.id, p.user_id, p.post_category_id, p.title, p.description, p.content, p.public_post, p.available,
                p.publish_date, p.expire_date,
                COALESCE(
                    (SELECT array_agg(t.title ORDER BY pt.creation_date, t.id)
                     FROM post_tags pt
                     JOIN tags t ON t.id = pt.tag_id
                     WHERE pt.post_id = p.id),
                    ARRAY[]::varchar[]) AS tags
            FROM posts p
            WHERE p.id = @id
            LIMIT 1;
            """;

        return await QuerySingleAsync(sql, postId, cancellationToken);
    }

    public async Task<BlogPost?> GetByIdForAuthorAsync(int postId, int authorUserId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                p.id, p.user_id, p.post_category_id, p.title, p.description, p.content, p.public_post, p.available,
                p.publish_date, p.expire_date,
                COALESCE(
                    (SELECT array_agg(t.title ORDER BY pt.creation_date, t.id)
                     FROM post_tags pt
                     JOIN tags t ON t.id = pt.tag_id
                     WHERE pt.post_id = p.id),
                    ARRAY[]::varchar[]) AS tags
            FROM posts p
            WHERE p.id = @id
              AND p.user_id = @author_user_id
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
            SELECT
                p.id, p.user_id, p.post_category_id, p.title, p.description, p.content, p.public_post, p.available,
                p.publish_date, p.expire_date,
                COALESCE(
                    (SELECT array_agg(t.title ORDER BY pt.creation_date, t.id)
                     FROM post_tags pt
                     JOIN tags t ON t.id = pt.tag_id
                     WHERE pt.post_id = p.id),
                    ARRAY[]::varchar[]) AS tags
            FROM posts p
            WHERE p.id = @id
              AND p.public_post = TRUE
              AND p.available = TRUE
              AND (p.publish_date IS NULL OR p.publish_date <= NOW())
              AND (p.expire_date IS NULL OR p.expire_date > NOW())
            LIMIT 1;
            """;

        return await QuerySingleAsync(sql, postId, cancellationToken);
    }

    public async Task<IReadOnlyList<BlogPost>> ListPublicReadAsync(CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                p.id, p.user_id, p.post_category_id, p.title, p.description, p.content, p.public_post, p.available,
                p.publish_date, p.expire_date,
                COALESCE(
                    (SELECT array_agg(t.title ORDER BY pt.creation_date, t.id)
                     FROM post_tags pt
                     JOIN tags t ON t.id = pt.tag_id
                     WHERE pt.post_id = p.id),
                    ARRAY[]::varchar[]) AS tags
            FROM posts p
            WHERE p.public_post = TRUE
              AND p.available = TRUE
              AND (p.publish_date IS NULL OR p.publish_date <= NOW())
              AND (p.expire_date IS NULL OR p.expire_date > NOW())
            ORDER BY p.id;
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

    public async Task<PaginatedBlogPostReadResult> ListPublicReadPageAsync(
        PostListPageRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        const string countSql = """
            SELECT COUNT(*) FROM (
                SELECT DISTINCT p.id
                FROM posts p
                INNER JOIN post_categories pc ON pc.id = p.post_category_id
                LEFT JOIN post_tags pt ON pt.post_id = p.id
                LEFT JOIN tags t ON t.id = pt.tag_id
                WHERE p.available = TRUE
                  AND (
                        (
                            @is_search = FALSE
                            AND p.public_post = TRUE
                            AND (p.publish_date IS NULL OR p.publish_date <= NOW())
                            AND (p.expire_date IS NULL OR p.expire_date > NOW())
                        )
                        OR (
                            @is_search = TRUE
                            AND (
                                (
                                    p.public_post = TRUE
                                    AND (p.publish_date IS NULL OR p.publish_date <= NOW())
                                    AND (p.expire_date IS NULL OR p.expire_date > NOW())
                                )
                                OR (@requesting_user_id IS NOT NULL AND p.user_id = @requesting_user_id)
                            )
                        )
                      )
                  AND (
                        @pattern IS NULL
                        OR p.title ILIKE @pattern
                        OR COALESCE(p.description, '') ILIKE @pattern
                        OR p.content ILIKE @pattern
                        OR pc.title ILIKE @pattern
                        OR COALESCE(t.title, '') ILIKE @pattern
                      )
            ) filtered_posts;
            """;

        const string pageSql = """
            SELECT
                p.id, p.user_id, p.post_category_id, p.title, p.description, p.content, p.public_post, p.available,
                p.publish_date, p.expire_date,
                COALESCE(
                    (SELECT array_agg(t2.title ORDER BY pt2.creation_date, t2.id)
                     FROM post_tags pt2
                     JOIN tags t2 ON t2.id = pt2.tag_id
                     WHERE pt2.post_id = p.id),
                    ARRAY[]::varchar[]) AS tags
            FROM posts p
            WHERE p.id IN (
                SELECT DISTINCT p2.id
                FROM posts p2
                INNER JOIN post_categories pc ON pc.id = p2.post_category_id
                LEFT JOIN post_tags pt ON pt.post_id = p2.id
                LEFT JOIN tags t ON t.id = pt.tag_id
                WHERE p2.available = TRUE
                  AND (
                        (
                            @is_search = FALSE
                            AND p2.public_post = TRUE
                            AND (p2.publish_date IS NULL OR p2.publish_date <= NOW())
                            AND (p2.expire_date IS NULL OR p2.expire_date > NOW())
                        )
                        OR (
                            @is_search = TRUE
                            AND (
                                (
                                    p2.public_post = TRUE
                                    AND (p2.publish_date IS NULL OR p2.publish_date <= NOW())
                                    AND (p2.expire_date IS NULL OR p2.expire_date > NOW())
                                )
                                OR (@requesting_user_id IS NOT NULL AND p2.user_id = @requesting_user_id)
                            )
                        )
                      )
                  AND (
                        @pattern IS NULL
                        OR p2.title ILIKE @pattern
                        OR COALESCE(p2.description, '') ILIKE @pattern
                        OR p2.content ILIKE @pattern
                        OR pc.title ILIKE @pattern
                        OR COALESCE(t.title, '') ILIKE @pattern
                      )
                ORDER BY p2.id
                LIMIT @limit
                OFFSET @offset
            )
            ORDER BY p.id;
            """;

        var posts = new List<BlogPost>();

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var countCommand = BuildPaginatedReadCommand(countSql, connection, request);
        var totalCount = Convert.ToInt32(await countCommand.ExecuteScalarAsync(cancellationToken));
        await using var itemsCommand = BuildPaginatedReadCommand(pageSql, connection, request);
        itemsCommand.Parameters.AddWithValue("limit", request.PageSize);
        itemsCommand.Parameters.AddWithValue("offset", request.Offset);
        await using var reader = await itemsCommand.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            posts.Add(MapPost(reader));
        }

        return new PaginatedBlogPostReadResult(posts, totalCount);
    }

    public async Task<IReadOnlyList<BlogPost>> SearchPublicReadAsync(
        string query,
        int? requestingUserId,
        CancellationToken cancellationToken = default)
    {
        var result = await ListPublicReadPageAsync(
            PostListPageRequest.Create(query, page: 1, pageSize: PostListPageRequest.MaxPageSize, requestingUserId: requestingUserId),
            cancellationToken);

        return result.Items;
    }

    public async Task<IReadOnlyList<BlogPost>> ListByAuthorAsync(int authorUserId, CancellationToken cancellationToken = default)
    {
        const string sql = """
            SELECT
                p.id, p.user_id, p.post_category_id, p.title, p.description, p.content, p.public_post, p.available,
                p.publish_date, p.expire_date,
                COALESCE(
                    (SELECT array_agg(t.title ORDER BY pt.creation_date, t.id)
                     FROM post_tags pt
                     JOIN tags t ON t.id = pt.tag_id
                     WHERE pt.post_id = p.id),
                    ARRAY[]::varchar[]) AS tags
            FROM posts p
            WHERE p.user_id = @author_user_id
            ORDER BY p.id;
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

        const string updatePostSql = """
            UPDATE posts
            SET
                title = @title,
                description = @description,
                content = @content,
                public_post = @public_post,
                available = @available,
                publish_date = @publish_date,
                expire_date = @expire_date,
                update_date = NOW(),
                update_user_id = @update_user_id
            WHERE id = @id
            RETURNING id, user_id, post_category_id, title, description, content, public_post, available, publish_date, expire_date;
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        BlogPost updatedPost;

        await using (var command = BuildPostUpdateCommand(updatePostSql, connection, transaction, post))
        {
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            if (!await reader.ReadAsync(cancellationToken))
            {
                throw new InvalidOperationException("The requested post was not found.");
            }

            updatedPost = MapPostFromReader(reader, post.Tags);
        }

        await using (var deleteTagsCommand = new NpgsqlCommand("DELETE FROM post_tags WHERE post_id = @post_id", connection, transaction))
        {
            deleteTagsCommand.Parameters.AddWithValue("post_id", post.Id);
            await deleteTagsCommand.ExecuteNonQueryAsync(cancellationToken);
        }

        await PersistTagsAsync(connection, transaction, post.Id, post.AuthorUserId, post.Tags, cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return updatedPost;
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

    private static async Task PersistTagsAsync(
        NpgsqlConnection connection,
        NpgsqlTransaction transaction,
        int postId,
        int authorUserId,
        IReadOnlyList<string> tags,
        CancellationToken cancellationToken)
    {
        if (tags.Count == 0)
        {
            return;
        }

        const string lookupTagSql = """
            SELECT id FROM tags WHERE lower(title) = lower(@title) LIMIT 1;
            """;

        const string insertTagSql = """
            INSERT INTO tags (title, created_by_user_id, creation_user_id, update_user_id)
            VALUES (@title, @created_by_user_id, @creation_user_id, @update_user_id)
            ON CONFLICT (title) DO NOTHING;
            """;

        const string insertPostTagSql = """
            INSERT INTO post_tags (post_id, tag_id, creation_user_id, update_user_id)
            VALUES (@post_id, @tag_id, @creation_user_id, @update_user_id)
            ON CONFLICT (post_id, tag_id) DO NOTHING;
            """;

        foreach (var tagTitle in tags)
        {
            int tagId;

            await using (var lookupCmd = new NpgsqlCommand(lookupTagSql, connection, transaction))
            {
                lookupCmd.Parameters.AddWithValue("title", tagTitle);
                var existing = await lookupCmd.ExecuteScalarAsync(cancellationToken);

                if (existing is not null)
                {
                    tagId = Convert.ToInt32(existing);
                }
                else
                {
                    await using (var insertTagCmd = new NpgsqlCommand(insertTagSql, connection, transaction))
                    {
                        insertTagCmd.Parameters.AddWithValue("title", tagTitle);
                        insertTagCmd.Parameters.AddWithValue("created_by_user_id", authorUserId);
                        insertTagCmd.Parameters.AddWithValue("creation_user_id", authorUserId);
                        insertTagCmd.Parameters.AddWithValue("update_user_id", authorUserId);
                        await insertTagCmd.ExecuteNonQueryAsync(cancellationToken);
                    }

                    await using var lookupAgainCmd = new NpgsqlCommand(lookupTagSql, connection, transaction);
                    lookupAgainCmd.Parameters.AddWithValue("title", tagTitle);
                    tagId = Convert.ToInt32(await lookupAgainCmd.ExecuteScalarAsync(cancellationToken));
                }
            }

            await using var insertPostTagCmd = new NpgsqlCommand(insertPostTagSql, connection, transaction);
            insertPostTagCmd.Parameters.AddWithValue("post_id", postId);
            insertPostTagCmd.Parameters.AddWithValue("tag_id", tagId);
            insertPostTagCmd.Parameters.AddWithValue("creation_user_id", authorUserId);
            insertPostTagCmd.Parameters.AddWithValue("update_user_id", authorUserId);
            await insertPostTagCmd.ExecuteNonQueryAsync(cancellationToken);
        }
    }

    private static NpgsqlCommand BuildPostInsertCommand(string sql, NpgsqlConnection connection, NpgsqlTransaction transaction, BlogPost post)
    {
        var command = new NpgsqlCommand(sql, connection, transaction);
        command.Parameters.AddWithValue("user_id", post.AuthorUserId);
        command.Parameters.AddWithValue("post_category_id", post.CategoryId);
        command.Parameters.AddWithValue("title", post.Title);
        command.Parameters.AddWithValue("description", (object?)post.Summary ?? DBNull.Value);
        command.Parameters.AddWithValue("content", post.Content);
        command.Parameters.AddWithValue("public_post", post.IsPublic);
        command.Parameters.AddWithValue("available", post.IsAvailable);
        command.Parameters.Add(new NpgsqlParameter<DateTimeOffset?>("publish_date", NpgsqlDbType.TimestampTz) { TypedValue = post.PublishDate });
        command.Parameters.Add(new NpgsqlParameter<DateTimeOffset?>("expire_date", NpgsqlDbType.TimestampTz) { TypedValue = post.ExpirationDate });
        command.Parameters.AddWithValue("creation_user_id", post.AuthorUserId);
        command.Parameters.AddWithValue("update_user_id", post.AuthorUserId);
        return command;
    }

    private static NpgsqlCommand BuildPostUpdateCommand(string sql, NpgsqlConnection connection, NpgsqlTransaction transaction, BlogPost post)
    {
        var command = new NpgsqlCommand(sql, connection, transaction);
        command.Parameters.AddWithValue("user_id", post.AuthorUserId);
        command.Parameters.AddWithValue("post_category_id", post.CategoryId);
        command.Parameters.AddWithValue("title", post.Title);
        command.Parameters.AddWithValue("description", (object?)post.Summary ?? DBNull.Value);
        command.Parameters.AddWithValue("content", post.Content);
        command.Parameters.AddWithValue("public_post", post.IsPublic);
        command.Parameters.AddWithValue("available", post.IsAvailable);
        command.Parameters.Add(new NpgsqlParameter<DateTimeOffset?>("publish_date", NpgsqlDbType.TimestampTz) { TypedValue = post.PublishDate });
        command.Parameters.Add(new NpgsqlParameter<DateTimeOffset?>("expire_date", NpgsqlDbType.TimestampTz) { TypedValue = post.ExpirationDate });
        command.Parameters.AddWithValue("update_user_id", post.AuthorUserId);
        command.Parameters.AddWithValue("id", post.Id);
        return command;
    }

    private static NpgsqlCommand BuildPaginatedReadCommand(string sql, NpgsqlConnection connection, PostListPageRequest request)
    {
        var command = new NpgsqlCommand(sql, connection);
        var pattern = request.Query is null ? null : $"%{request.Query}%";
        command.Parameters.Add(new NpgsqlParameter<string?>("pattern", NpgsqlDbType.Text) { TypedValue = pattern });
        command.Parameters.Add(new NpgsqlParameter<int?>("requesting_user_id", NpgsqlDbType.Integer) { TypedValue = request.RequestingUserId });
        command.Parameters.Add(new NpgsqlParameter<bool>("is_search", NpgsqlDbType.Boolean) { TypedValue = request.Query is not null });
        return command;
    }

    private static BlogPost MapPost(NpgsqlDataReader reader)
    {
        var idxPublishDate = reader.GetOrdinal("publish_date");
        var idxExpireDate = reader.GetOrdinal("expire_date");
        var idxTags = reader.GetOrdinal("tags");

        var tags = reader.IsDBNull(idxTags)
            ? Array.Empty<string>()
            : (IReadOnlyList<string>)(reader.GetFieldValue<string[]>(idxTags));

        return BlogPost.Rehydrate(
            id: reader.GetInt32(reader.GetOrdinal("id")),
            authorUserId: reader.GetInt32(reader.GetOrdinal("user_id")),
            categoryId: reader.GetInt32(reader.GetOrdinal("post_category_id")),
            title: reader.GetString(reader.GetOrdinal("title")),
            summary: reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
            content: reader.GetString(reader.GetOrdinal("content")),
            isPublic: reader.GetBoolean(reader.GetOrdinal("public_post")),
            isAvailable: reader.GetBoolean(reader.GetOrdinal("available")),
            tags: tags,
            publishDate: reader.IsDBNull(idxPublishDate) ? null : reader.GetFieldValue<DateTimeOffset>(idxPublishDate),
            expirationDate: reader.IsDBNull(idxExpireDate) ? null : reader.GetFieldValue<DateTimeOffset>(idxExpireDate));
    }

    private static BlogPost MapPostFromReader(NpgsqlDataReader reader, IReadOnlyList<string> tags)
    {
        var idxPublishDate = reader.GetOrdinal("publish_date");
        var idxExpireDate = reader.GetOrdinal("expire_date");

        return BlogPost.Rehydrate(
            id: reader.GetInt32(reader.GetOrdinal("id")),
            authorUserId: reader.GetInt32(reader.GetOrdinal("user_id")),
            categoryId: reader.GetInt32(reader.GetOrdinal("post_category_id")),
            title: reader.GetString(reader.GetOrdinal("title")),
            summary: reader.IsDBNull(reader.GetOrdinal("description")) ? null : reader.GetString(reader.GetOrdinal("description")),
            content: reader.GetString(reader.GetOrdinal("content")),
            isPublic: reader.GetBoolean(reader.GetOrdinal("public_post")),
            isAvailable: reader.GetBoolean(reader.GetOrdinal("available")),
            tags: tags,
            publishDate: reader.IsDBNull(idxPublishDate) ? null : reader.GetFieldValue<DateTimeOffset>(idxPublishDate),
            expirationDate: reader.IsDBNull(idxExpireDate) ? null : reader.GetFieldValue<DateTimeOffset>(idxExpireDate));
    }
}
