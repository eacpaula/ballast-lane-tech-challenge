using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Categories;
using BlogPlatform.Domain.Posts;

namespace BlogPlatform.Application.Tests.Posts;

public class CreateBlogPostHandlerTagTests
{
    [Fact]
    public async Task HandleAsync_WithTags_NormalizesAndForwardsTagsToRepository()
    {
        var postRepository = new FakePostRepository();
        var handler = new CreateBlogPostHandler(postRepository, new AlwaysAvailableCategoryRepository());

        var command = new CreateBlogPostCommand(
            AuthenticatedUserId: 7,
            CategoryId: 3,
            Title: "Tagged Post",
            Summary: null,
            Content: "Content",
            Tags: new[] { "  rust  ", "cloud-native", "rust" });

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Tags.Count);
        Assert.Equal("rust", result.Tags[0]);
        Assert.Equal("cloud-native", result.Tags[1]);
        Assert.Equal(2, postRepository.CreatedPost!.Tags.Count);
    }

    [Fact]
    public async Task HandleAsync_WithNullTags_ReturnsEmptyTagsOnResult()
    {
        var postRepository = new FakePostRepository();
        var handler = new CreateBlogPostHandler(postRepository, new AlwaysAvailableCategoryRepository());

        var command = new CreateBlogPostCommand(
            AuthenticatedUserId: 7,
            CategoryId: 3,
            Title: "No Tags Post",
            Summary: null,
            Content: "Content",
            Tags: null);

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Tags);
        Assert.Empty(postRepository.CreatedPost!.Tags);
    }

    [Fact]
    public async Task HandleAsync_WithAllEmptyTags_ReturnsEmptyTagsOnResult()
    {
        var postRepository = new FakePostRepository();
        var handler = new CreateBlogPostHandler(postRepository, new AlwaysAvailableCategoryRepository());

        var command = new CreateBlogPostCommand(
            AuthenticatedUserId: 7,
            CategoryId: 3,
            Title: "Empty Tags Post",
            Summary: null,
            Content: "Content",
            Tags: new[] { "", "  ", " " });

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Tags);
    }

    private sealed class FakePostRepository : IPostRepository
    {
        public BlogPost? CreatedPost { get; private set; }

        public Task<BlogPost> CreateAsync(BlogPost post, CancellationToken cancellationToken = default)
        {
            var savedPost = BlogPost.Rehydrate(
                id: 101,
                authorUserId: post.AuthorUserId,
                categoryId: post.CategoryId,
                title: post.Title,
                summary: post.Summary,
                content: post.Content,
                tags: post.Tags);

            CreatedPost = savedPost;
            return Task.FromResult(savedPost);
        }

        public Task DeleteAsync(int postId, CancellationToken cancellationToken = default) => throw new NotSupportedException();

        public Task<BlogPost?> GetByIdAsync(int postId, CancellationToken cancellationToken = default) =>
            Task.FromResult<BlogPost?>(null);

        public Task<BlogPost?> GetByIdForAuthorAsync(int postId, int authorUserId, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<BlogPost?> GetPublicReadByIdAsync(int postId, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<IReadOnlyList<BlogPost>> ListByAuthorAsync(int authorUserId, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<IReadOnlyList<BlogPost>> ListPublicReadAsync(CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<BlogPost> UpdateAsync(BlogPost post, CancellationToken cancellationToken = default) =>
            Task.FromResult(post);
    }

    private sealed class AlwaysAvailableCategoryRepository : ICategoryRepository
    {
        public Task<bool> ExistsAndAvailableAsync(int categoryId, CancellationToken cancellationToken = default) =>
            Task.FromResult(true);

        public Task<PaginatedCategoryResult<PostCategory>> ListAllAsync(CategoryPageRequest request, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<PaginatedCategoryResult<PostCategory>> ListAvailableAsync(CategoryPageRequest request, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<PostCategory> CreateAsync(PostCategory category, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<PostCategory> DeactivateAsync(PostCategory category, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<PostCategory?> GetByIdAsync(int categoryId, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<bool> TitleExistsAsync(string title, int? excludingCategoryId = null, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<PostCategory> UpdateAsync(PostCategory category, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }
}
