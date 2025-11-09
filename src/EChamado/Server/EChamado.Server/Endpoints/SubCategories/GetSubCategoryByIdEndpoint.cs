using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Categories.ViewModels;
using EChamado.Server.Application.UseCases.SubCategories.Queries;
using EChamado.Shared.Responses;
using Paramore.Brighter;

namespace EChamado.Server.Endpoints.SubCategories;

public class GetSubCategoryByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/{id:guid}", HandleAsync)
            .WithName("Obter subcategoria por ID")
            .Produces<BaseResult<SubCategoryViewModel>>();

    private static async Task<IResult> HandleAsync(
        IAmACommandProcessor commandProcessor,
        Guid id)
    {
        var query = new GetSubCategoryByIdQuery(id);
        var result = await commandProcessor.Send(query);

        if (result.Success)
            return TypedResults.Ok(result);

        return TypedResults.BadRequest(result);
    }
}
