using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Departments.Commands;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace EChamado.Server.Endpoints.Departments;

public class UpdateStatusDepartmentEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    => app.MapPatch("/update-status", HandleAsync)
        .WithName("Desativa uma lista de departamentos")
        .WithSummary("Desativa uma lista de departamentos")
        .WithDescription("Desativa uma lista de departamentos")
        .WithOrder(8)
        .Produces<BaseResult>();

    private static async Task<IResult> HandleAsync(
        [FromServices] IAmACommandProcessor commandProcessor,
        [FromBody] UpdateStatusDepartmentCommand command)
    {
        await commandProcessor.SendAsync(command);
        var result = command.Result;

        if (result.Success)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.BadRequest(result);
    }
}