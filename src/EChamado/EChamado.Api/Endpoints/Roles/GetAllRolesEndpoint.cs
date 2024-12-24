using EChamado.Api.Common.Api;
using EChamado.Application.Features.Roles.Queries;
using EChamado.Application.Features.Roles.ViewModels;
using EChamado.Core.Responses;
using MediatR;

namespace EChamado.Api.Endpoints.Roles;

public class GetAllRolesEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    => app.MapGet("/", HandleAsync)
        .WithName("Obtem todas as roles")
        .WithSummary("Obtem todas as roles")
        .WithDescription("Obtem todas as roles")
        .WithOrder(1)
        .Produces<BaseResultList<RolesViewModel>>();

    private static async Task<IResult> HandleAsync(
        IMediator mediator)
    {
        var result = await mediator.Send(new GetAllRolesQuery());

        if (result.Success)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.BadRequest(result);
    }
}