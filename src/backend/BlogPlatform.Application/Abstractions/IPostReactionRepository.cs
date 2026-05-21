using BlogPlatform.Domain.Reactions;

namespace BlogPlatform.Application.Abstractions;

public interface IPostReactionRepository
{
    Task<PostReaction> CreateAsync(PostReaction reaction, CancellationToken cancellationToken = default);
}
