namespace BlogPlatform.Application.Abstractions;

public interface ICategoryRepository
{
    Task<bool> ExistsAndAvailableAsync(int categoryId, CancellationToken cancellationToken = default);
}
