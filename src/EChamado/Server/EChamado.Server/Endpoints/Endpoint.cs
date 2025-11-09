using EChamado.Server.Common.Api;
using EChamado.Server.Endpoints.Auth;
using EChamado.Server.Endpoints.Categories;
using EChamado.Server.Endpoints.Comments;
using EChamado.Server.Endpoints.Departments;
using EChamado.Server.Endpoints.Orders;
using EChamado.Server.Endpoints.OrderTypes;
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

        endpoints.MapGroup("v1/categories")
            .WithTags("Category")
            .RequireAuthorization()
            .MapEndpoint<SearchCategoriesEndpoint>();

        endpoints.MapGroup("v1/category")
            .WithTags("Category")
            .RequireAuthorization()
            .MapEndpoint<GetCategoryByIdEndpoint>()
            .MapEndpoint<CreateCategoryEndpoint>()
            .MapEndpoint<UpdateCategoryEndpoint>()
            .MapEndpoint<DeleteCategoryEndpoint>();

        endpoints.MapGroup("v1/subcategories")
            .WithTags("SubCategory")
            .RequireAuthorization()
            .MapEndpoint<SearchSubCategoriesEndpoint>();

        endpoints.MapGroup("v1/subcategory")
            .WithTags("SubCategory")
            .RequireAuthorization()
            .MapEndpoint<GetSubCategoryByIdEndpoint>()
            .MapEndpoint<CreateSubCategoryEndpoint>()
            .MapEndpoint<UpdateSubCategoryEndpoint>()
            .MapEndpoint<DeleteSubCategoryEndpoint>();

        endpoints.MapGroup("v1/ordertypes")
            .WithTags("OrderType")
            .RequireAuthorization()
            .MapEndpoint<SearchOrderTypesEndpoint>();

        endpoints.MapGroup("v1/ordertype")
            .WithTags("OrderType")
            .RequireAuthorization()
            .MapEndpoint<GetOrderTypeByIdEndpoint>()
            .MapEndpoint<CreateOrderTypeEndpoint>()
            .MapEndpoint<UpdateOrderTypeEndpoint>()
            .MapEndpoint<DeleteOrderTypeEndpoint>();

        endpoints.MapGroup("v1/statustypes")
            .WithTags("StatusType")
            .RequireAuthorization()
            .MapEndpoint<SearchStatusTypesEndpoint>();

        endpoints.MapGroup("v1/statustype")
            .WithTags("StatusType")
            .RequireAuthorization()
            .MapEndpoint<GetStatusTypeByIdEndpoint>()
            .MapEndpoint<CreateStatusTypeEndpoint>()
            .MapEndpoint<UpdateStatusTypeEndpoint>()
            .MapEndpoint<DeleteStatusTypeEndpoint>();

        endpoints.MapGroup("v1/orders")
            .WithTags("Order")
            .RequireAuthorization()
            .MapEndpoint<SearchOrdersEndpoint>();

        endpoints.MapGroup("v1/order")
            .WithTags("Order")
            .RequireAuthorization()
            .MapEndpoint<GetOrderByIdEndpoint>()
            .MapEndpoint<CreateOrderEndpoint>()
            .MapEndpoint<UpdateOrderEndpoint>()
            .MapEndpoint<AssignOrderEndpoint>()
            .MapEndpoint<CloseOrderEndpoint>()
            .MapEndpoint<ChangeStatusOrderEndpoint>()
            .MapEndpoint<CreateCommentEndpoint>()
            .MapEndpoint<GetCommentsByOrderIdEndpoint>();

        endpoints.MapGroup("v1")
            .WithTags("Comment")
            .RequireAuthorization()
            .MapEndpoint<DeleteCommentEndpoint>();

    }

    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}