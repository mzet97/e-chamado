using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Roles.Queries;
using EChamado.Server.Application.UseCases.Roles.ViewModels;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using Paramore.Brighter;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.Roles;

public class GetAllRolesEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    => app.MapGet("/", HandleAsync)
        .WithName("Obtem todas as roles")
        .WithSummary("Obtem todas as roles")
        .WithDescription("Obtem todas as roles")
        .WithOrder(1)
        .Produces<BaseResultList<RolesViewModel>>();

    private static async Task<IResult> HandleAsync(
        [FromServices] IAmACommandProcessor commandProcessor)
    {
        try
        {
            var query = new GetAllRolesQuery();
            await commandProcessor.SendAsync(query);

            return query.Result.Success
                ? TypedResults.Ok(query.Result)
                : TypedResults.BadRequest(query.Result);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(new BaseResultList<RolesViewModel>(
                new List<RolesViewModel>(),
                null,
                false,
                $"Erro interno: {ex.Message}"));
        }
    }
}