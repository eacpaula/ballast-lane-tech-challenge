using BlogPlatform.Api.Configuration;
using BlogPlatform.Api.Errors;
using BlogPlatform.Api.Extensions;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var jwtSettings = JwtAuthenticationSettings.FromConfiguration(builder.Configuration);
var localCorsSettings = LocalCorsSettings.FromConfiguration(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalFrontend", policy =>
    {
        policy.WithOrigins(localCorsSettings.AllowedOrigins.ToArray())
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BlogPlatform API",
        Version = "v1",
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Provide a valid JWT bearer token.",
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme,
                },
            },
            Array.Empty<string>()
        },
    });
});

builder.Services.AddBlogPlatformApplication();
builder.Services.AddBlogPlatformInfrastructure(builder.Configuration, jwtSettings);
builder.Services.AddBlogPlatformAuthentication(jwtSettings);

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var problem = context.CreateProblemDetails(
            statusCode: StatusCodes.Status500InternalServerError,
            title: "Unexpected Error",
            detail: "An unexpected error occurred while processing the request.");

        await context.WriteProblemDetailsAsync(problem);
    });
});

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("LocalFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program;
