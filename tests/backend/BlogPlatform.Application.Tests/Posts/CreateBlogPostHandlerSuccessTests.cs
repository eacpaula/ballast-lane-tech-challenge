using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Categories;
using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Tests.Posts;

public class CreateBlogPostHandlerSuccessTests
{
    [Fact]
    public async Task HandleAsync_WithValidCommand_CreatesPostAndReturnsSuccess()
    {
        var categoryRepository = new FakeCategoryRepository(isAvailable: true);
        var postRepository = new FakePostRepository();
        var handler = new CreateBlogPostHandler(postRepository, categoryRepository);

        var command = new CreateBlogPostCommand(
            AuthenticatedUserId: 7,
            CategoryId: 3,
            Title: "Valid title",
            Summary: "Summary",
            Content: "Useful content");

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(101, result.PostId);
        Assert.Equal(7, result.AuthorUserId);
        Assert.Equal(3, result.CategoryId);
        Assert.Equal("Valid title", result.Title);
        Assert.Equal("Summary", result.Summary);
        Assert.Equal("Useful content", result.Content);
        Assert.Single(postRepository.CreatedPosts);
        Assert.Equal(7, postRepository.CreatedPosts[0].AuthorUserId);
    }

    private sealed class FakeCategoryRepository(bool isAvailable) : ICategoryRepository
    {
        public Task<IReadOnlyList<PostCategory>> ListAllAsync(CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<IReadOnlyList<PostCategory>> ListAvailableAsync(CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<PostCategory> CreateAsync(PostCategory category, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<PostCategory> DeactivateAsync(PostCategory category, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<bool> ExistsAndAvailableAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(isAvailable);
        }

        public Task<PostCategory?> GetByIdAsync(int categoryId, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<bool> TitleExistsAsync(string title, int? excludingCategoryId = null, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<PostCategory> UpdateAsync(PostCategory category, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }
    }

    private sealed class FakePostRepository : IPostRepository
    {
        public List<BlogPost> CreatedPosts { get; } = [];

        public Task<BlogPost> CreateAsync(BlogPost post, CancellationToken cancellationToken = default)
        {
            var savedPost = BlogPost.Rehydrate(
                id: 101,
                authorUserId: post.AuthorUserId,
                categoryId: post.CategoryId,
                title: post.Title,
                summary: post.Summary,
                content: post.Content);

            CreatedPosts.Add(savedPost);

            return Task.FromResult(savedPost);
        }

        public Task DeleteAsync(int postId, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<BlogPost?> GetByIdAsync(int postId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<BlogPost?>(null);
        }

        public Task<BlogPost?> GetByIdForAuthorAsync(int postId, int authorUserId, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<BlogPost?> GetPublicReadByIdAsync(int postId, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<IReadOnlyList<BlogPost>> ListByAuthorAsync(int authorUserId, CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<IReadOnlyList<BlogPost>> ListPublicReadAsync(CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }

        public Task<BlogPost> UpdateAsync(BlogPost post, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(post);
        }
    }
}
