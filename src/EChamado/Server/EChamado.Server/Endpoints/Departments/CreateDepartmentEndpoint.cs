using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Departments.Commands;
using EChamado.Server.Endpoints.Departments.DTOs;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using Paramore.Brighter;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.Departments;

public class CreateDepartmentEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/", HandleAsync)
            .WithName("Criar um novo departamento")
            .Produces<BaseResult<Guid>>();

    private static async Task<IResult> HandleAsync(
        [FromServices] IAmACommandProcessor commandProcessor,
        [FromBody] CreateDepartmentRequest request)
    {
        try
        {
            var command = request.ToCommand();
            await commandProcessor.SendAsync(command);

            var result = command.Result;

            if (result.Success)
                return TypedResults.Ok(result);

            return TypedResults.BadRequest(result);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(new BaseResult<Guid>(
                data: Guid.Empty,
                success: false,
                message: $"Erro interno: {ex.Message}"));
        }
    }
}
