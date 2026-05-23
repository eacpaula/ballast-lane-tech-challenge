using System.Net.Http.Headers;
using System.Net.Http.Json;
using BlogPlatform.Api.Contracts.Auth;
using BlogPlatform.Application.Abstractions;
using BlogPlatform.Infrastructure.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BlogPlatform.Api.Tests.TestSupport;

public sealed class BlogPlatformApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public BlogPlatformApiFactory()
    {
        Database = new PostgreSqlApiTestDatabase(PostgreSqlConnectionSettings.FromConfiguration(new ConfigurationBuilder().Build()));
    }

    public PostgreSqlApiTestDatabase Database { get; }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IPostListCache>();
            services.AddSingleton<IPostListCache, InMemoryPostListCache>();
        });
    }

    public async Task InitializeAsync()
    {
        await Database.VerifyConnectivityAsync();
        await Database.ResetToSeedStateAsync();
    }

    public new Task DisposeAsync() => Task.CompletedTask;

    public async Task<HttpClient> CreateAuthenticatedClientAsync(string email, string password)
    {
        var client = CreateClient();
        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginUserRequest(email, password));
        response.EnsureSuccessStatusCode();
        var login = await response.Content.ReadFromJsonAsync<AuthResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login!.AuthenticationPayload);
        return client;
    }
}
