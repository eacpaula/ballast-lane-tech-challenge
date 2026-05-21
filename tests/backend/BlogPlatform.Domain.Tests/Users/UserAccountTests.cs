using BlogPlatform.Domain.Users;

namespace BlogPlatform.Domain.Tests.Users;

public class UserAccountTests
{
    [Fact]
    public void CreateNew_WithValidData_ReturnsUserAccount()
    {
        var user = UserAccount.CreateNew(
            nameOrUsername: "  alice  ",
            email: "  alice@example.com  ",
            passwordHash: "  hashed-password  ");

        Assert.Equal(0, user.Id);
        Assert.Equal("alice", user.NameOrUsername);
        Assert.Equal("alice@example.com", user.Email);
        Assert.Equal("hashed-password", user.PasswordHash);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateNew_WithBlankNameOrUsername_ThrowsArgumentException(string nameOrUsername)
    {
        void Action() => UserAccount.CreateNew(
            nameOrUsername: nameOrUsername,
            email: "alice@example.com",
            passwordHash: "hashed-password");

        var exception = Assert.Throws<ArgumentException>(Action);

        Assert.Equal("nameOrUsername", exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("alice")]
    [InlineData("alice@")]
    public void CreateNew_WithInvalidEmail_ThrowsArgumentException(string email)
    {
        void Action() => UserAccount.CreateNew(
            nameOrUsername: "alice",
            email: email,
            passwordHash: "hashed-password");

        var exception = Assert.Throws<ArgumentException>(Action);

        Assert.Equal("email", exception.ParamName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void CreateNew_WithBlankPasswordHash_ThrowsArgumentException(string passwordHash)
    {
        void Action() => UserAccount.CreateNew(
            nameOrUsername: "alice",
            email: "alice@example.com",
            passwordHash: passwordHash);

        var exception = Assert.Throws<ArgumentException>(Action);

        Assert.Equal("passwordHash", exception.ParamName);
    }
}
