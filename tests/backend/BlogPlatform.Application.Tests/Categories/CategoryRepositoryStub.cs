using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Posts;
using BlogPlatform.Domain.Categories;

namespace BlogPlatform.Application.Tests.Categories;

internal sealed class CategoryRepositoryStub : ICategoryRepository
{
    public Func<CategoryPageRequest, PaginatedCategoryResult<PostCategory>>? ListAllHandler { get; init; }
    public Func<CategoryPageRequest, PaginatedCategoryResult<PostCategory>>? ListAvailableHandler { get; init; }
    public Func<string, int?, bool>? TitleExistsHandler { get; init; }
    public Func<int, PostCategory?>? GetByIdHandler { get; init; }
    public Func<PostCategory, PostCategory>? CreateHandler { get; init; }
    public Func<PostCategory, PostCategory>? UpdateHandler { get; init; }
    public Func<PostCategory, PostCategory>? DeactivateHandler { get; init; }
    public Func<int, bool>? ExistsAndAvailableHandler { get; init; }

    public bool ListAllWasCalled { get; private set; }
    public bool ListAvailableWasCalled { get; private set; }
    public bool CreateWasCalled { get; private set; }
    public bool UpdateWasCalled { get; private set; }
    public bool DeactivateWasCalled { get; private set; }
    public PostCategory? LastCreatedCategory { get; private set; }
    public PostCategory? LastUpdatedCategory { get; private set; }
    public PostCategory? LastDeactivatedCategory { get; private set; }

    public Task<PaginatedCategoryResult<PostCategory>> ListAllAsync(
        CategoryPageRequest request,
        CancellationToken cancellationToken = default)
    {
        ListAllWasCalled = true;
        return Task.FromResult(ListAllHandler?.Invoke(request) ?? new PaginatedCategoryResult<PostCategory>([], request.Page, request.PageSize, 0));
    }

    public Task<PaginatedCategoryResult<PostCategory>> ListAvailableAsync(
        CategoryPageRequest request,
        CancellationToken cancellationToken = default)
    {
        ListAvailableWasCalled = true;
        return Task.FromResult(ListAvailableHandler?.Invoke(request) ?? new PaginatedCategoryResult<PostCategory>([], request.Page, request.PageSize, 0));
    }

    public Task<bool> ExistsAndAvailableAsync(int categoryId, CancellationToken cancellationToken = default)
        => Task.FromResult(ExistsAndAvailableHandler?.Invoke(categoryId) ?? false);

    public Task<bool> TitleExistsAsync(string title, int? excludingCategoryId = null, CancellationToken cancellationToken = default)
        => Task.FromResult(TitleExistsHandler?.Invoke(title, excludingCategoryId) ?? false);

    public Task<PostCategory> CreateAsync(PostCategory category, CancellationToken cancellationToken = default)
    {
        CreateWasCalled = true;
        LastCreatedCategory = category;
        return Task.FromResult(CreateHandler?.Invoke(category) ?? category);
    }

    public Task<PostCategory?> GetByIdAsync(int categoryId, CancellationToken cancellationToken = default)
        => Task.FromResult(GetByIdHandler?.Invoke(categoryId));

    public Task<PostCategory> UpdateAsync(PostCategory category, CancellationToken cancellationToken = default)
    {
        UpdateWasCalled = true;
        LastUpdatedCategory = category;
        return Task.FromResult(UpdateHandler?.Invoke(category) ?? category);
    }

    public Task<PostCategory> DeactivateAsync(PostCategory category, CancellationToken cancellationToken = default)
    {
        DeactivateWasCalled = true;
        LastDeactivatedCategory = category;
        return Task.FromResult(DeactivateHandler?.Invoke(category) ?? category);
    }
}
