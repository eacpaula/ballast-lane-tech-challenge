using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BlogPlatform.Application.Abstractions;
using BlogPlatform.Domain.Users;
using BlogPlatform.Infrastructure.Configuration;
using BlogPlatform.Infrastructure.Data;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

namespace BlogPlatform.Infrastructure.Security;

public sealed class JwtAuthenticationPayloadFactory : IAuthenticationPayloadFactory
{
    private readonly NpgsqlConnectionFactory _connectionFactory;
    private readonly JwtTokenSettings _settings;

    public JwtAuthenticationPayloadFactory(
        NpgsqlConnectionFactory connectionFactory,
        JwtTokenSettings settings)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    public string Create(UserAccount user)
    {
        ArgumentNullException.ThrowIfNull(user);
        return CreateAsync(user).GetAwaiter().GetResult();
    }

    private async Task<string> CreateAsync(UserAccount user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.NameOrUsername),
            new(ClaimTypes.Email, user.Email),
        };

        foreach (var role in await LoadRolesAsync(user.Id))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.ExpirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<IReadOnlyList<string>> LoadRolesAsync(int userId)
    {
        const string sql = """
            SELECT r.title
            FROM roles r
            INNER JOIN user_roles ur ON ur.role_id = r.id
            WHERE ur.user_id = @user_id
            ORDER BY r.id;
            """;

        var roles = new List<string>();
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("user_id", userId);
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            roles.Add(reader.GetString(0));
        }

        return roles;
    }
}
