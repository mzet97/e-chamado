using EChamado.Server.Application.UseCases.Categories.Queries;
using EChamado.Server.Application.UseCases.Categories.ViewModels;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Endpoints.Categories;

public class GetCategoryByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/{id:guid}", HandleAsync)
            .WithName("Obter categoria por ID")
            .Produces<BaseResult<CategoryViewModel>>();

    private static async Task<IResult> HandleAsync(
        IMediator mediator,
        Guid id)
    {
        var query = new GetCategoryByIdQuery(id);
        var result = await mediator.Send(query);

        if (result.Success)
            return TypedResults.Ok(result);

        return TypedResults.BadRequest(result);
    }
}
