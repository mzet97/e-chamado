using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Users.Queries;
using EChamado.Server.Application.UseCases.Users.ViewModels;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using Paramore.Brighter;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.Users;

public class GetByIdUserEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/{id:guid}", HandleAsync)
            .WithName("Buscar usuário por ID")
            .Produces<BaseResult<ApplicationUserViewModel>>();

    public static async Task<IResult> HandleAsync(
        Guid id,
        [FromServices] IAmACommandProcessor commandProcessor)
    {
        try
        {
            var query = new GetByIdUserQuery(id);
            await commandProcessor.SendAsync(query);

            return query.Result.Success
                ? TypedResults.Ok(query.Result)
                : TypedResults.NotFound(query.Result);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(new BaseResult<ApplicationUserViewModel>(
                data: null,
                success: false,
                message: $"Erro interno: {ex.Message}"));
        }
    }
}
