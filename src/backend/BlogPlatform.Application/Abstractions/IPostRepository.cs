using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Abstractions;

public interface IPostRepository
{
    Task<BlogPost> CreateAsync(BlogPost post, CancellationToken cancellationToken = default);
}
