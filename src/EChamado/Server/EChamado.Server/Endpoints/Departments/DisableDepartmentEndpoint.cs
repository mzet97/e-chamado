using EChamado.Server.Application.UseCases.Departments.Commands;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.Departments;

public class DisableDepartmentEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    => app.MapDelete("/disable/{id}", HandleAsync)
        .WithName("Desativa uma departamento pelo id")
        .WithSummary("Desativa uma departamento pelo id")
        .WithDescription("Desativa uma departamento pelo id")
        .WithOrder(7)
        .Produces<BaseResult>();

    private static async Task<IResult> HandleAsync(
        IMediator mediator,
        [FromRoute] Guid id)
    {
        var result = await mediator.Send(new DisableDepartmentCommand(id));

        if (result.Success)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.BadRequest(result);
    }
}