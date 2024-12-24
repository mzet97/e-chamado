using EChamado.Api.Common.Api;
using EChamado.Application.Features.Departments.Commands;
using EChamado.Core.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Api.Endpoints.Departments;

public class DeleteListDepartmentEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    => app.MapDelete("/", HandleAsync)
        .WithName("Deleta uma lista de departamentos")
        .WithSummary("Deleta uma lista de departamentos")
        .WithDescription("Deleta uma lista de departamentos")
        .WithOrder(6)
        .Produces<BaseResult>();

    private static async Task<IResult> HandleAsync(
        IMediator mediator,
        [FromBody] DeleteListDepartmentCommand command)
    {
        var result = await mediator.Send(command);

        if (result.Success)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.BadRequest(result);
    }
}