using System.Net;
using System.Net.Http.Json;
using BlogPlatform.Api.Contracts.Auth;
using BlogPlatform.Api.Tests.TestSupport;
using Microsoft.AspNetCore.Mvc;

namespace BlogPlatform.Api.Tests.Auth;

public sealed class AuthLoginTests : IClassFixture<BlogPlatformApiFactory>
{
    private readonly BlogPlatformApiFactory _factory;

    public AuthLoginTests(BlogPlatformApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Login_ReturnsAuthenticationPayloadForValidCredentials()
    {
        await _factory.Database.ResetToSeedStateAsync();
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginUserRequest("user@blogplatform.local", "User123!"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(body);
        Assert.False(string.IsNullOrWhiteSpace(body!.AuthenticationPayload));
    }

    [Fact]
    public async Task Login_ReturnsUnauthorizedForInvalidCredentials()
    {
        await _factory.Database.ResetToSeedStateAsync();
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginUserRequest("user@blogplatform.local", "wrong-password"));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(body);
    }
}
