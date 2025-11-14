using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Roles.Queries;
using EChamado.Server.Application.UseCases.Roles.ViewModels;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace EChamado.Server.Endpoints.Roles;

public class GetRoleByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    => app.MapGet("/{id:guid}", HandleAsync)
        .WithName("Obtem role pelo id")
        .WithSummary("Obtem role pelo id")
        .WithDescription("Obtem role pelo id")
        .WithOrder(2)
        .Produces<BaseResult<RolesViewModel>>();

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromServices] IAmACommandProcessor commandProcessor)
    {
        var result = await commandProcessor.SendWithResultAsync(new GetRoleByIdQuery(id));

        if (result.Success)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.BadRequest(result);
    }
}