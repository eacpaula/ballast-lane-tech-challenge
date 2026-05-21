using System.Security.Claims;
using System.Text;
using BlogPlatform.Api.Configuration;
using BlogPlatform.Api.Errors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace BlogPlatform.Api.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddBlogPlatformAuthentication(
        this IServiceCollection services,
        JwtAuthenticationSettings jwtSettings)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(jwtSettings);

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SigningKey));

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = ClaimTypes.Name,
                    RoleClaimType = ClaimTypes.Role,
                };

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        var problem = context.HttpContext.CreateProblemDetails(
                            statusCode: StatusCodes.Status401Unauthorized,
                            title: "Unauthorized",
                            detail: "Authentication is required to access this resource.");

                        await context.HttpContext.WriteProblemDetailsAsync(problem);
                    },
                    OnForbidden = async context =>
                    {
                        var problem = context.HttpContext.CreateProblemDetails(
                            statusCode: StatusCodes.Status403Forbidden,
                            title: "Forbidden",
                            detail: "You do not have permission to access this resource.");

                        await context.HttpContext.WriteProblemDetailsAsync(problem);
                    },
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("AdministratorOnly", policy => policy.RequireRole("Administrator"));
        });

        return services;
    }
}
