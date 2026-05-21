using BlogPlatform.Application.Abstractions;
using BlogPlatform.Application.Users;
using BlogPlatform.Domain.Users;

namespace BlogPlatform.Application.Tests.Users;

public class RegisterUserHandlerSuccessTests
{
    [Fact]
    public async Task HandleAsync_WithValidInput_HashesPasswordPersistsUserAndReturnsSuccess()
    {
        var userRepository = new TrackingUserRepository();
        var passwordSecurityService = new TrackingPasswordSecurityService();
        var handler = new RegisterUserHandler(userRepository, passwordSecurityService);

        var command = new RegisterUserCommand(
            NameOrUsername: "alice",
            Email: "alice@example.com",
            Password: "StrongPassword123!");

        var result = await handler.HandleAsync(command);

        Assert.True(result.IsSuccess);
        Assert.Equal(101, result.UserId);
        Assert.Equal("alice", result.NameOrUsername);
        Assert.Equal("alice@example.com", result.Email);
        Assert.Null(result.ErrorCode);
        Assert.Null(result.ErrorMessage);
        Assert.Equal(["StrongPassword123!"], passwordSecurityService.HashedPasswords);
        Assert.NotNull(userRepository.CreatedUser);
        Assert.Equal("hashed::StrongPassword123!", userRepository.CreatedUser!.PasswordHash);
        Assert.NotEqual("StrongPassword123!", userRepository.CreatedUser.PasswordHash);
    }

    private sealed class TrackingUserRepository : IUserRepository
    {
        public UserAccount? CreatedUser { get; private set; }

        public Task<UserAccount> CreateAsync(UserAccount user, CancellationToken cancellationToken = default)
        {
            CreatedUser = UserAccount.Rehydrate(
                id: 101,
                nameOrUsername: user.NameOrUsername,
                email: user.Email,
                passwordHash: user.PasswordHash);

            return Task.FromResult(CreatedUser);
        }

        public Task<UserAccount?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<UserAccount?>(null);
        }
    }

    private sealed class TrackingPasswordSecurityService : IPasswordSecurityService
    {
        public List<string> HashedPasswords { get; } = [];

        public string HashPassword(string password)
        {
            HashedPasswords.Add(password);
            return $"hashed::{password}";
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            throw new NotSupportedException();
        }
    }
}
