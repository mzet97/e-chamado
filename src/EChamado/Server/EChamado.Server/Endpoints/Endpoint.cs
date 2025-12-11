using EChamado.Server.Common.Api;
using EChamado.Server.Endpoints.AI;
using EChamado.Server.Endpoints.Categories;
using EChamado.Server.Endpoints.Comments;
using EChamado.Server.Endpoints.Departments;
using EChamado.Server.Endpoints.OrderTypes;
using EChamado.Server.Endpoints.Orders;
using EChamado.Server.Endpoints.Roles;
using EChamado.Server.Endpoints.StatusTypes;
using EChamado.Server.Endpoints.SubCategories;
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
             .MapGet("/health-check", async ([FromServices] ILogger<Program> logger) =>
             {
                 logger.LogInformation("Health Check executed.");
                 return Results.Ok(new { message = "OK" });
             });

        endpoints
            .MapGroup("/")
            .WithTags("Cache Redis")
            .MapGet("/cached-endpoint", async (HttpContext context) =>
            {
                context.Response.Headers["Cache-Control"] = "public, max-age=300";
                return Results.Ok(new { Message = "Este é um exemplo de cache", Timestamp = DateTime.UtcNow });
            }).CacheOutput("DefaultPolicy");

        // VERSION 1 ENDPOINTS (V1)
        // TODOS OS ENDPOINTS ESTÃO NA V1 AGORA

        // Auth v1 - MIGRADO PARA OPENIDDICT
        // Use /connect/token no Auth Server (porta 7132) para autenticação
        // Exemplos: ver arquivos test-openiddict-login.sh, test-openiddict-login.ps1, test-openiddict-login.py

        // Roles v1
        endpoints.MapGroup("v1/role")
           .WithTags("role")
           .RequireAuthorization()
           .MapEndpoint<GetAllRolesEndpoint>()
           .MapEndpoint<GetRoleByIdEndpoint>()
           .MapEndpoint<GetRoleByNameEndpoint>()
           .MapEndpoint<CreateRoleEndpoint>()
           .MapEndpoint<UpdateRoleEndpoint>()
           .MapEndpoint<DeleteRoleEndpoint>();

        // Users v1
        endpoints.MapGroup("v1/users")
            .WithTags("user")
            .RequireAuthorization()
            .MapEndpoint<GetAllUsersEndpoint>()
            .MapEndpoint<GetByIdUserEndpoint>()
            .MapEndpoint<GetByEmailUserEndpoint>();

        // Departments v1
        endpoints.MapGroup("v1/departments")
            .WithTags("Department")
            .RequireAuthorization()
            .MapEndpoint<SearchDepartmentEndpoint>()
            .MapEndpoint<GridifyDepartmentsEndpoint>()
            .MapEndpoint<DeleteDepartmentEndpoint>()
            .MapEndpoint<UpdateStatusDepartmentEndpoint>()
            .MapEndpoint<GetByIdDepartmentEndpoint>()
            .MapEndpoint<CreateDepartmentEndpoint>()
            .MapEndpoint<UpdateDepartmentEndpoint>();

        // Categories v1
        endpoints.MapGroup("v1/categories")
            .WithTags("Category")
            .RequireAuthorization() // TEMPORÁRIAMENTE DESABILITADO
            .MapEndpoint<GetCategoryByIdEndpoint>()
            .MapEndpoint<GridifyCategoriesEndpoint>()
            .MapEndpoint<CreateCategoryEndpoint>()
            .MapEndpoint<UpdateCategoryEndpoint>()
            .MapEndpoint<DeleteCategoryEndpoint>()
            .MapEndpoint<SearchCategoriesEndpoint>();

        // SubCategories v1
        endpoints.MapGroup("v1/subcategories")
            .WithTags("SubCategory")
            .RequireAuthorization()
            .MapEndpoint<SearchSubCategoriesEndpoint>()
            .MapEndpoint<GetSubCategoryByIdEndpoint>()
            .MapEndpoint<CreateSubCategoryEndpoint>()
            .MapEndpoint<UpdateSubCategoryEndpoint>()
            .MapEndpoint<DeleteSubCategoryEndpoint>();


        // OrderTypes v1
        endpoints.MapGroup("v1/ordertypes")
            .WithTags("OrderType")
            .RequireAuthorization()
            .MapEndpoint<SearchOrderTypesEndpoint>()
            .MapEndpoint<GridifyOrderTypesEndpoint>()
            .MapEndpoint<GetOrderTypeByIdEndpoint>()
            .MapEndpoint<CreateOrderTypeEndpoint>()
            .MapEndpoint<UpdateOrderTypeEndpoint>()
            .MapEndpoint<DeleteOrderTypeEndpoint>();

        // StatusTypes v1
        endpoints.MapGroup("v1/statustypes")
            .WithTags("StatusType")
            .RequireAuthorization()
            .MapEndpoint<SearchStatusTypesEndpoint>()
            .MapEndpoint<GridifyStatusTypesEndpoint>()
            .MapEndpoint<GetStatusTypeByIdEndpoint>()
            .MapEndpoint<CreateStatusTypeEndpoint>()
            .MapEndpoint<UpdateStatusTypeEndpoint>()
            .MapEndpoint<DeleteStatusTypeEndpoint>();

        // Orders v1
        endpoints.MapGroup("v1/orders")
            .WithTags("Order")
            .RequireAuthorization()
            .MapEndpoint<SearchOrdersEndpoint>()
            .MapEndpoint<GridifyOrdersEndpoint>()
            .MapEndpoint<GetOrderByIdEndpoint>()
            .MapEndpoint<CreateOrderEndpoint>()
            .MapEndpoint<UpdateOrderEndpoint>()
            .MapEndpoint<AssignOrderEndpoint>()
            .MapEndpoint<ChangeStatusOrderEndpoint>()
            .MapEndpoint<CloseOrderEndpoint>();

        // Comments v1
        endpoints.MapGroup("v1/comments")
            .WithTags("Comment")
            .RequireAuthorization()
            .MapEndpoint<CreateCommentEndpoint>()
            .MapEndpoint<GetCommentsByOrderIdEndpoint>()
            .MapEndpoint<DeleteCommentEndpoint>();

        // AI v1 - Natural Language to Gridify conversion
        endpoints.MapGroup("v1/ai")
            .WithTags("AI")
            .RequireAuthorization()
            .MapEndpoint<ConvertNLToGridifyEndpoint>();
    }

    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}
