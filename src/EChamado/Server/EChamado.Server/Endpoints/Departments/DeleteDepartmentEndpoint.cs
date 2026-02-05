using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Departments.Commands;
using EChamado.Server.Endpoints.Departments.DTOs;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using Paramore.Brighter;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.Departments;

public class DeleteDepartmentEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapDelete("/{id:guid}", HandleAsync)
            .WithName("Deletar departamento")
            .Produces<BaseResult>();

    private static async Task<IResult> HandleAsync(
        Guid id,
        [FromServices] IAmACommandProcessor commandProcessor)
    {
        try
        {
            var request = new DeleteDepartmentRequest { Id = id };
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
