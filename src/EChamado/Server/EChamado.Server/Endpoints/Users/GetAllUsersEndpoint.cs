using EChamado.Server.Application.UseCases.Users.Queries;
using EChamado.Server.Application.UseCases.Users.ViewModels;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Endpoints.Users;

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