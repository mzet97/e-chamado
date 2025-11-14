using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Auth.Commands;
using EChamado.Server.Endpoints.Auth.DTOs;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using EChamado.Shared.ViewModels.Auth;
using Paramore.Brighter;
using Microsoft.AspNetCore.Mvc;

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
        [FromServices] IAmACommandProcessor commandProcessor,
        [FromBody] LoginRequestDto request)
    {
        try
        {
            var command = request.ToCommand();
            await commandProcessor.SendAsync(command);
            var result = command.Result;

            if (result.Success)
            {
                return TypedResults.Ok(result);
            }

            return TypedResults.BadRequest(result);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(new BaseResult<LoginResponseViewModel?>(
                data: null,
                success: false,
                message: $"Erro interno: {ex.Message}"));
        }
    }
}
