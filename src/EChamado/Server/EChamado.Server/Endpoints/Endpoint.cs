using EChamado.Server.Common.Api;
using EChamado.Server.Endpoints.Auth;
using EChamado.Server.Endpoints.Departments;
using EChamado.Server.Endpoints.Roles;
using EChamado.Server.Endpoints.Users;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints;

public static class Endpoint
{
    public static void MapEndpoints(this WebApplication app)
    {
        var endpoints = app
            .MapGroup("");

        //app.MapGroup("/account").MapIdentityApi<ApplicationUser>();

        endpoints.MapGroup("/")
             .WithTags("Health Check")
             .MapGet("/health-check", ([FromServices] ILogger<Program> logger) =>
             {
                 logger.LogInformation("Health Check executed.");
                 return Results.Ok(new { message = "OK" });
             });

        endpoints
            .MapGroup("/")
            .WithTags("Cache Redis")
            .MapGet("/cached-endpoint", (HttpContext context) =>
            {
                context.Response.Headers["Cache-Control"] = "public, max-age=300";
                return Results.Ok(new { Message = "Este é um exemplo de cache", Timestamp = DateTime.UtcNow });
            }).CacheOutput("DefaultPolicy");

        endpoints.MapGroup("v1/auth")
            .WithTags("auth")
            .MapEndpoint<RegisterUserEndpoint>()
            .MapEndpoint<LoginUserEndpoint>();

        endpoints.MapGroup("v1/roles")
           .WithTags("role")
           .RequireAuthorization()
           .MapEndpoint<GetAllRolesEndpoint>();

        endpoints.MapGroup("v1/role")
           .WithTags("role")
           .RequireAuthorization()
           .MapEndpoint<GetRoleByIdEndpoint>()
           .MapEndpoint<GetRoleByNameEndpoint>()
           .MapEndpoint<CreateRoleEndpoint>()
           .MapEndpoint<UpdateRoleEndpoint>()
           .MapEndpoint<DeleteRoleEndpoint>();

        endpoints.MapGroup("v1/users")
          .WithTags("user")
          .RequireAuthorization()
          .MapEndpoint<GetAllUsersEndpoint>();

        endpoints.MapGroup("v1/user")
           .WithTags("user")
           .RequireAuthorization()
           .MapEndpoint<GetByIdUserEndpoint>()
           .MapEndpoint<GetByEmailUserEndpoint>();

        endpoints.MapGroup("v1/departments")
            .WithTags("Department")
            .RequireAuthorization()
            .MapEndpoint<SearchDepartmentEndpoint>()
            .MapEndpoint<DeletesDepartmentEndpoint>()
            .MapEndpoint<UpdateStatusDepartmentEndpoint>();

        endpoints.MapGroup("v1/department")
            .WithTags("Department")
            .RequireAuthorization()
            .MapEndpoint<GetByIdDepartmentEndpoint>()
            .MapEndpoint<CreateDepartmentEndpoint>()
            .MapEndpoint<UpdateDepartmentEndpoint>();
            

    }

    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}