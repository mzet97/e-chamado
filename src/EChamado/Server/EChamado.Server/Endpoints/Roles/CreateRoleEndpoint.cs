using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Roles.Commands;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using Paramore.Brighter;

namespace EChamado.Server.Endpoints.Roles;

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
        IAmACommandProcessor commandProcessor,
        CreateRoleCommand command)
    {

        var result = await commandProcessor.Send(command);

        if (result.Success)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.BadRequest(result);
    }
}