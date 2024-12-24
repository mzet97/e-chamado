using EChamado.Api.Common.Api;
using EChamado.Application.Features.Roles.Commands;
using EChamado.Core.Responses;
using MediatR;

namespace EChamado.Api.Endpoints.Roles;

public class CreateRoleEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    => app.MapPost("/", HandleAsync)
        .WithName("Criar uma nova role")
        .WithSummary("Criar uma nova role")
        .WithDescription("Criar uma nova role")
        .WithOrder(4)
        .Produces<BaseResult<Guid>>();

    private static async Task<IResult> HandleAsync(
        IMediator mediator,
        CreateRoleCommand command)
    {

        var result = await mediator.Send(command);

        if (result.Success)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.BadRequest(result);
    }
}