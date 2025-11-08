using EChamado.Server.Application.UseCases.Orders.Commands;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Endpoints.Orders;

public class UpdateOrderEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    => app.MapPut("/{id}", HandleAsync)
        .WithName("Atualizar chamado")
        .WithSummary("Atualizar chamado")
        .WithDescription("Atualizar chamado")
        .WithOrder(4)
        .Produces<BaseResult>();

    private static async Task<IResult> HandleAsync(
        IMediator mediator,
        Guid id,
        UpdateOrderRequest request)
    {
        var command = new UpdateOrderCommand(
            id,
            request.Title,
            request.Description,
            request.TypeId,
            request.CategoryId,
            request.SubCategoryId,
            request.DepartmentId,
            request.DueDate
        );

        var result = await mediator.Send(command);

        if (result.Success)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.BadRequest(result);
    }
}

public record UpdateOrderRequest(
    string Title,
    string Description,
    Guid TypeId,
    Guid? CategoryId,
    Guid? SubCategoryId,
    Guid? DepartmentId,
    DateTime? DueDate
);
