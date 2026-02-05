using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Users.Queries;
using EChamado.Server.Application.UseCases.Users.ViewModels;
using EChamado.Server.Endpoints.Users.DTOs;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using Paramore.Brighter;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.Users;

public class GetAllUsersEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/", HandleAsync)
            .WithName("Buscar usuários")
            .Produces<BaseResultList<ApplicationUserViewModel>>();

    public static async Task<IResult> HandleAsync(
        [AsParameters] GetAllUsersRequest request,
        [FromServices] IAmACommandProcessor commandProcessor)
    {
        try
        {
            var query = request.ToQuery();
            await commandProcessor.SendAsync(query);

            return query.Result.Success
                ? TypedResults.Ok(query.Result)
                : TypedResults.BadRequest(query.Result);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(new BaseResultList<ApplicationUserViewModel>(
                data: new List<ApplicationUserViewModel>(),
                pagedResult: new PagedResult { CurrentPage = 0, PageCount = 0, PageSize = 10, RowCount = 0 },
                success: false,
                message: $"Erro interno: {ex.Message}"));
        }
    }
}
