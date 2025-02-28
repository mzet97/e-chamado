using EChamado.Server.Application.UseCases.Auth.Commands;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using EChamado.Shared.ViewModels.Auth;
using MediatR;

namespace EChamado.Server.Endpoints.Auth;

public class LoginUserEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/login", HandleAsync)
            .WithName("Login: login in application")
            .WithSummary("Faz o login")
            .WithDescription("Faz o login")
            .WithOrder(1)
            .Produces<BaseResult<LoginResponseViewModel?>>();

    private static async Task<IResult> HandleAsync(
        IMediator mediator,
        LoginUserCommand command)
    {

        var result = await mediator.Send(command);

        if (result.Success)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.BadRequest(result);
    }
}
