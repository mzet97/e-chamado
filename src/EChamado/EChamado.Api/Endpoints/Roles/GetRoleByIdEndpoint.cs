using EChamado.Api.Common.Api;
using EChamado.Application.Features.Roles.Queries;
using EChamado.Application.Features.Roles.ViewModels;
using EChamado.Core.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Api.Endpoints.Roles;

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
        IMediator mediator,
        [FromRoute]  Guid id)
    {
        var result = await mediator.Send(new GetRoleByIdQuery(id));

        if (result.Success)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.BadRequest(result);
    }
}