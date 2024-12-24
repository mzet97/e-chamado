using EChamado.Api.Common.Api;
using EChamado.Application.Features.Departments.Commands;
using EChamado.Core.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Api.Endpoints.Departments;

public class UpdateDepartmentEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    => app.MapPut("/{id}", HandleAsync)
        .WithName("Atualiza um departamento")
        .WithSummary("Atualiza um departamento")
        .WithDescription("Atualiza um departamento")
        .WithOrder(4)
        .Produces<BaseResult<Guid>>();

    private static async Task<IResult> HandleAsync(
        IMediator mediator,
        [FromRoute] Guid id,
        [FromBody] UpdateDepartmentCommand command)
    {

        if(id != command.Id)
        {
            return TypedResults.BadRequest("Id da rota e Id do corpo da requisição não são iguais");
        }

        var result = await mediator.Send(command);

        if (result.Success)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.BadRequest(result);
    }
}