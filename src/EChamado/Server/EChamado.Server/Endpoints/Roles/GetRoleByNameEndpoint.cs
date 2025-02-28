using EChamado.Server.Application.UseCases.Roles.Queries;
using EChamado.Server.Application.UseCases.Roles.ViewModels;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
        IMediator mediator,
        [FromRoute] string name)
    {
        var result = await mediator.Send(new GetRoleByNameQuery(name));

        if (result.Success)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.BadRequest(result);
    }
}