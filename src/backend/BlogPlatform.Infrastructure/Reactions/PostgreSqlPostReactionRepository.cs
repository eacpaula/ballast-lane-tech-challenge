using BlogPlatform.Application.Abstractions;
using BlogPlatform.Domain.Reactions;
using BlogPlatform.Infrastructure.Data;
using Npgsql;

namespace BlogPlatform.Infrastructure.Reactions;

public sealed class PostgreSqlPostReactionRepository : IPostReactionRepository
{
    private readonly NpgsqlConnectionFactory _connectionFactory;

    public PostgreSqlPostReactionRepository(NpgsqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<PostReaction> CreateAsync(PostReaction reaction, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reaction);

        const string sql = """
            INSERT INTO post_reactions (
                post_id,
                user_id,
                visitor_identifier,
                reaction_type,
                creation_user_id,
                update_user_id)
            VALUES (
                @post_id,
                @user_id,
                @visitor_identifier,
                @reaction_type,
                @creation_user_id,
                @update_user_id)
            RETURNING post_id, user_id, visitor_identifier, reaction_type;
            """;

        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("post_id", reaction.PostId);
        command.Parameters.AddWithValue("user_id", (object?)reaction.UserId ?? DBNull.Value);
        command.Parameters.AddWithValue("visitor_identifier", (object?)reaction.VisitorIdentifier ?? DBNull.Value);
        command.Parameters.AddWithValue("reaction_type", reaction.ReactionType.Value.ToLowerInvariant());
        command.Parameters.AddWithValue("creation_user_id", (object?)reaction.UserId ?? DBNull.Value);
        command.Parameters.AddWithValue("update_user_id", (object?)reaction.UserId ?? DBNull.Value);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        await reader.ReadAsync(cancellationToken);

        return PostReaction.Create(
            reader.GetInt32(0),
            ReactionType.Create(reader.GetString(3)),
            ReactionActor.Create(reader.IsDBNull(1) ? null : reader.GetInt32(1), reader.IsDBNull(2) ? null : reader.GetString(2)));
    }
}
