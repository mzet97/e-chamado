using EChamado.Api.Common.Api;
using EChamado.Application.Features.Departments.Commands;
using EChamado.Application.Features.Departments.ViewModels;
using EChamado.Core.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Api.Endpoints.Departments;

public class DeleteDepartmentEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    => app.MapDelete("/{id}", HandleAsync)
        .WithName("Deleta uma departamento pelo id")
        .WithSummary("Deleta uma departamento pelo id")
        .WithDescription("Deleta uma departamento pelo id")
        .WithOrder(5)
        .Produces<BaseResult>();

    private static async Task<IResult> HandleAsync(
        IMediator mediator,
        [FromRoute] Guid id)
    {
        var result = await mediator.Send(new DeleteDepartmentCommand(id));

        if (result.Success)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.BadRequest(result);
    }
}