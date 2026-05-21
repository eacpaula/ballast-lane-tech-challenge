using System.Net;
using System.Net.Http.Json;
using BlogPlatform.Api.Contracts.Auth;
using BlogPlatform.Api.Tests.TestSupport;
using Microsoft.AspNetCore.Mvc;

namespace BlogPlatform.Api.Tests.Auth;

public sealed class AuthRegisterFailureTests : IClassFixture<BlogPlatformApiFactory>
{
    private readonly BlogPlatformApiFactory _factory;

    public AuthRegisterFailureTests(BlogPlatformApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Register_ReturnsConflictForDuplicateEmail()
    {
        await _factory.Database.ResetToSeedStateAsync();
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/register", new RegisterUserRequest("duplicate-user", "user@blogplatform.local", "Password123!"));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(body);
    }

    [Fact]
    public async Task Register_ReturnsBadRequestForInvalidPayload()
    {
        await _factory.Database.ResetToSeedStateAsync();
        using var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/register", new RegisterUserRequest("", "invalid-email", ""));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(body);
    }
}
