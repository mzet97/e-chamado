using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.StatusTypes.Queries;
using EChamado.Server.Application.UseCases.StatusTypes.ViewModels;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using Paramore.Brighter;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.StatusTypes;

public class GetStatusTypeByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/{id:guid}", HandleAsync)
            .WithName("Obter status de chamado por ID")
            .Produces<BaseResult<StatusTypeViewModel>>();

    private static async Task<IResult> HandleAsync(
        Guid id,
        [FromServices] IAmACommandProcessor commandProcessor)
    {
        try
        {
            var query = new GetStatusTypeByIdQuery(id);
            await commandProcessor.SendAsync(query);

            return query.Result.Success
                ? TypedResults.Ok(query.Result)
                : TypedResults.NotFound(query.Result);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(new BaseResult<StatusTypeViewModel>(
                data: null,
                success: false,
                message: $"Erro interno: {ex.Message}"));
        }
    }
}
