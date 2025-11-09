using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Orders.Commands;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using Paramore.Brighter;
using System.Security.Claims;

namespace EChamado.Server.Endpoints.Orders;

public class CreateOrderEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    => app.MapPost("/", HandleAsync)
        .WithName("Criar novo chamado")
        .WithSummary("Criar novo chamado")
        .WithDescription("Criar novo chamado")
        .WithOrder(3)
        .Produces<BaseResult<Guid>>();

    private static async Task<IResult> HandleAsync(
        IAmACommandProcessor commandProcessor,
        HttpContext httpContext,
        CreateOrderRequest request)
    {
        var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userEmail = httpContext.User.FindFirst(ClaimTypes.Email)?.Value;

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userEmail))
        {
            return TypedResults.Unauthorized();
        }

        var command = new CreateOrderCommand(
            request.Title,
            request.Description,
            request.TypeId,
            request.CategoryId,
            request.SubCategoryId,
            request.DepartmentId,
            request.DueDate,
            Guid.Parse(userId),
            userEmail
        );

        var result = await commandProcessor.Send(command);

        if (result.Success)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.BadRequest(result);
    }
}

public record CreateOrderRequest(
    string Title,
    string Description,
    Guid TypeId,
    Guid? CategoryId,
    Guid? SubCategoryId,
    Guid? DepartmentId,
    DateTime? DueDate
);
