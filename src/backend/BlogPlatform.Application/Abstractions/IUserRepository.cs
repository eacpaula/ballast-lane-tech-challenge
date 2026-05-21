using BlogPlatform.Domain.Users;

namespace BlogPlatform.Application.Abstractions;

public interface IUserRepository
{
    Task<UserAccount> CreateAsync(UserAccount user, CancellationToken cancellationToken = default);

    Task<UserAccount?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
