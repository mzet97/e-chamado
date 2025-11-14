using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Users.Queries;
using EChamado.Server.Application.UseCases.Users.ViewModels;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using Paramore.Brighter;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.Users;

public class GetByEmailUserEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/email/{email}", HandleAsync)
            .WithName("Buscar usuário por email")
            .Produces<BaseResult<ApplicationUserViewModel>>();

    public static async Task<IResult> HandleAsync(
        string email,
        [FromServices] IAmACommandProcessor commandProcessor)
    {
        try
        {
            var query = new GetByEmailUserQuery(email);
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
