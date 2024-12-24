using EChamado.Api.Common.Api;
using EChamado.Api.Endpoints.Auth;
using EChamado.Api.Endpoints.Departments;
using EChamado.Api.Endpoints.Roles;
using EChamado.Api.Endpoints.Users;
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

        endpoints.MapGroup("v1/roles")
           .WithTags("role")
           .MapEndpoint<GetAllRolesEndpoint>();

        endpoints.MapGroup("v1/role")
           .WithTags("role")
           .MapEndpoint<GetRoleByIdEndpoint>()
           .MapEndpoint<GetRoleByNameEndpoint>()
           .MapEndpoint<CreateRoleEndpoint>()
           .MapEndpoint<UpdateRoleEndpoint>()
           .MapEndpoint<DeleteRoleEndpoint>();

        endpoints.MapGroup("v1/users")
          .WithTags("user")
          .MapEndpoint<GetAllUsersEndpoint>();

        endpoints.MapGroup("v1/user")
           .WithTags("user")
           .MapEndpoint<GetByIdUserEndpoint>();

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