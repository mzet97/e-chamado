using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Roles.Commands;
using EChamado.Server.Endpoints.Roles.DTOs;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using Paramore.Brighter;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.Roles;

public class UpdateRoleEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("/{id}", HandleAsync)
            .WithName("Atualizar uma role")
            .Produces<BaseResult>();

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromServices] IAmACommandProcessor commandProcessor,
        [FromBody] UpdateRoleRequest request)
    {
        try
        {
            // Atribui o ID da rota ao request
            request.Id = id;

            var command = request.ToCommand();
            await commandProcessor.SendAsync(command);

            var result = command.Result;

            if (result.Success)
                return TypedResults.Ok(result);

            return TypedResults.BadRequest(result);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(new BaseResult(
                success: false,
                message: $"Erro interno: {ex.Message}"));
        }
    }
}
