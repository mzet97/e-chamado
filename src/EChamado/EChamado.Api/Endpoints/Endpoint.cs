using EChamado.Api.Common.Api;
using EChamado.Api.Endpoints.Auth;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Api.Endpoints;

public static class Endpoint
{
    public static void MapEndpoints(this WebApplication app)
    {
        var endpoints = app
            .MapGroup("");

        endpoints.MapGroup("/")
             .WithTags("Health Check")
             .MapGet("/", async ([FromServices] ILogger<Program> logger) =>
             {
                 logger.LogInformation("Health Check executed.");
                 return Results.Ok(new { message = "OK" });
             });


        endpoints.MapGroup("v1/auth")
            .WithTags("auth")
            .MapEndpoint<RegisterUserEndpoint>()
            .MapEndpoint<LoginUserEndpoint>();

    }

    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}