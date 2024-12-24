using EChamado.Api.Common.Api;
using EChamado.Application.Features.Users.Queries;
using EChamado.Application.Features.Users.ViewModels;
using EChamado.Core.Responses;
using MediatR;

namespace EChamado.Api.Endpoints.Users;

public class GetAllUsersEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    => app.MapGet("/", HandleAsync)
        .WithName("Obtem todas os usuarios")
        .WithSummary("Obtem todas os usuarios")
        .WithDescription("Obtem todas os usuarios")
        .WithOrder(1)
        .Produces<BaseResultList<ApplicationUserViewModel>>();

    private static async Task<IResult> HandleAsync(
        IMediator mediator)
    {
        var result = await mediator.Send(new GetAllUsersQuery());

        if (result.Success)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.BadRequest(result);
    }
}