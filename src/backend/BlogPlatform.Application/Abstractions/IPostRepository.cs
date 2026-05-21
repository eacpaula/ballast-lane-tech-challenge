using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Abstractions;

public interface IPostRepository
{
    Task<BlogPost> CreateAsync(BlogPost post, CancellationToken cancellationToken = default);

    Task<BlogPost?> GetByIdAsync(int postId, CancellationToken cancellationToken = default);

    Task<BlogPost> UpdateAsync(BlogPost post, CancellationToken cancellationToken = default);
}
