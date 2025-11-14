using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Categories.Queries;
using EChamado.Server.Application.UseCases.Categories.ViewModels;
using EChamado.Shared.Responses;
using Paramore.Brighter;
using EChamado.Server.Common.Api;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.Categories;

public class GetCategoryByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/{id:guid}", HandleAsync)
            .WithName("Obter categoria por ID")
            .Produces<BaseResult<CategoryViewModel>>();

    private static async Task<IResult> HandleAsync(
        [FromServices] IAmACommandProcessor commandProcessor,
        Guid id)
    {
        var query = new GetCategoryByIdQuery(id);
        var result = await commandProcessor.SendWithResultAsync(query);

        if (result.Success)
            return TypedResults.Ok(result);

        return TypedResults.BadRequest(result);
    }
}
