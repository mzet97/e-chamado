using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Roles.Queries;
using EChamado.Server.Application.UseCases.Roles.ViewModels;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace EChamado.Server.Endpoints.Roles;

public class GetRoleByNameEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    => app.MapGet("/{name:alpha}", HandleAsync)
        .WithName("Obtem role pelo nome")
        .WithSummary("Obtem role pelo nome")
        .WithDescription("Obtem role pelo nome")
        .WithOrder(3)
        .Produces<BaseResult<RolesViewModel>>();

    private static async Task<IResult> HandleAsync(
        [FromRoute] string name,
        [FromServices] IAmACommandProcessor commandProcessor)
    {
        var result = await commandProcessor.SendWithResultAsync(new GetRoleByNameQuery(name));

        if (result.Success)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.BadRequest(result);
    }
}