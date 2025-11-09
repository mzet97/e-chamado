using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Roles.Commands;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace EChamado.Server.Endpoints.Roles;

public class DeleteRoleEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    => app.MapDelete("/{id}", HandleAsync)
        .WithName("Deleta uma role pelo id")
        .WithSummary("Deleta uma role pelo id")
        .WithDescription("Deleta uma role pelo id")
        .WithOrder(6)
        .Produces<BaseResult>();

    private static async Task<IResult> HandleAsync(
        IAmACommandProcessor commandProcessor,
        [FromRoute] Guid id)
    {
        var result = await commandProcessor.Send(new DeleteRoleCommand(id));

        if (result.Success)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.BadRequest(result);
    }
}