using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Departments.Commands;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace EChamado.Server.Endpoints.Departments;

public class DeletesDepartmentEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    => app.MapDelete("/", HandleAsync)
        .WithName("Deleta uma lista de departamentos")
        .WithSummary("Deleta uma lista de departamentos")
        .WithDescription("Deleta uma lista de departamentos")
        .WithOrder(6)
        .Produces<BaseResult>();

    private static async Task<IResult> HandleAsync(
        IAmACommandProcessor commandProcessor,
        [FromQuery(Name = "ids")] Guid[] ids)
    {
        if (ids == null || !ids.Any())
        {
            return TypedResults.BadRequest(new BaseResult(false, "Nenhum ID foi fornecido."));
        }

        var result = await commandProcessor.Send(new DeletesDepartmentCommand(ids));

        if (result.Success)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.BadRequest(result);
    }
}