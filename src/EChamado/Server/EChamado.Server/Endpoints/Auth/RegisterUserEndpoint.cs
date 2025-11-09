using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Auth.Commands;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using EChamado.Shared.ViewModels.Auth;
using Paramore.Brighter;

namespace EChamado.Server.Endpoints.Auth;

public class RegisterUserEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
         => app.MapPost("/register", HandleAsync)
             .WithName("Register: Register a new user")
             .WithSummary("Criar um novo usuario")
             .WithDescription("Criar um novo usuario")
             .WithOrder(1)
             .Produces<BaseResult<LoginResponseViewModel?>>();

    private static async Task<IResult> HandleAsync(
        IAmACommandProcessor commandProcessor,
        RegisterUserCommand command)
    {

        var result = await commandProcessor.Send(command);

        if (result.Success)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.BadRequest(result);
    }
}
