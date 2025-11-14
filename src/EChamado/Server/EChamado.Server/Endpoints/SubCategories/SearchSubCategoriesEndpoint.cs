using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.SubCategories.Queries;
using EChamado.Server.Endpoints.SubCategories.DTOs;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using EChamado.Shared.ViewModels;
using Paramore.Brighter;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.SubCategories;

public class SearchSubCategoriesEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/", HandleAsync)
            .WithName("Buscar subcategorias")
            .Produces<BaseResultList<BaseViewModel>>();

    public static async Task<IResult> HandleAsync(
        [AsParameters] SearchSubCategoriesRequest request,
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
            return TypedResults.BadRequest(new BaseResultList<BaseViewModel>(
                data: new List<BaseViewModel>(),
                pagedResult: new PagedResult { CurrentPage = 0, PageCount = 0, PageSize = 10, RowCount = 0 },
                success: false,
                message: $"Erro interno: {ex.Message}"));
        }
    }
}
