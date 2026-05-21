using System.Net;
using System.Net.Http.Json;
using BlogPlatform.Api.Contracts.Auth;
using BlogPlatform.Api.Tests.TestSupport;

namespace BlogPlatform.Api.Tests.Auth;

public sealed class AuthRegisterSuccessTests : IClassFixture<BlogPlatformApiFactory>
{
    private readonly BlogPlatformApiFactory _factory;

    public AuthRegisterSuccessTests(BlogPlatformApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Register_ReturnsCreatedUserForValidRequest()
    {
        await _factory.Database.ResetToSeedStateAsync();
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/register", new RegisterUserRequest("new-user", "new-user@example.test", "Password123!"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(body);
        Assert.True(body!.UserId > 0);
        Assert.Equal("new-user@example.test", body.Email);
    }
}
