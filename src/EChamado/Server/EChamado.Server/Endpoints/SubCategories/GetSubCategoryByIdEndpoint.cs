using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Endpoints.SubCategories.DTOs;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using EChamado.Shared.ViewModels;
using Paramore.Brighter;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.SubCategories;

public class GetSubCategoryByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/{id:guid}", HandleAsync)
            .WithName("Buscar subcategoria por ID")
            .Produces<BaseResult<BaseViewModel>>();

    public static async Task<IResult> HandleAsync(
        Guid id,
        [FromServices] IAmACommandProcessor commandProcessor)
    {
        try
        {
            var requestDto = new GetSubCategoryByIdRequest { Id = id };
            var query = requestDto.ToQuery();
            await commandProcessor.SendAsync(query);

            return query.Result.Success
                ? TypedResults.Ok(query.Result)
                : TypedResults.NotFound(query.Result);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(new BaseResult<BaseViewModel>(
                data: null,
                success: false,
                message: $"Erro interno: {ex.Message}"));
        }
    }
}
