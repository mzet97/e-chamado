using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Roles.Queries;
using EChamado.Server.Application.UseCases.Roles.ViewModels;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace EChamado.Server.Endpoints.Roles;

public class GetRoleByNameEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    => app.MapGet("/{name:alpha}", HandleAsync)
        .WithName("Obtem role pelo nome")
        .WithSummary("Obtem role pelo nome")
        .WithDescription("Obtem role pelo nome")
        .WithOrder(3)
        .Produces<BaseResult<RolesViewModel>>();

    private static async Task<IResult> HandleAsync(
        string name,
        [FromServices] IAmACommandProcessor commandProcessor)
    {
        try
        {
            var query = new GetRoleByNameQuery(name);
            await commandProcessor.SendAsync(query);

            return query.Result.Success
                ? TypedResults.Ok(query.Result)
                : TypedResults.NotFound(query.Result);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(new BaseResult<RolesViewModel>(
                data: null,
                success: false,
                message: $"Erro interno: {ex.Message}"));
        }
    }
}