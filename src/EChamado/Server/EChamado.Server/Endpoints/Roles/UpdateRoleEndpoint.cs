using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Roles.Commands;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace EChamado.Server.Endpoints.Roles;

public class UpdateRoleEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    => app.MapPut("/{id:guid}", HandleAsync)
        .WithName("Atualiza um role")
        .WithSummary("Atualiza um role")
        .WithDescription("Atualiza um role")
        .WithOrder(5)
        .Produces<BaseResult>();

    private static async Task<IResult> HandleAsync(
        IAmACommandProcessor commandProcessor,
        [FromRoute] Guid id,
        [FromBody] UpdateRoleCommand command)
    {

        if (id != command.Id)
        {
            return TypedResults.BadRequest("Id da rota e Id do corpo da requisição não são iguais");
        }

        var result = await commandProcessor.Send(command);

        if (result.Success)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.BadRequest(result);
    }
}