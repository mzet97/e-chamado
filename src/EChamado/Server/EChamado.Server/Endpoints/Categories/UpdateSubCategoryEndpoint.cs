using EChamado.Server.Application.UseCases.Categories.Commands;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Endpoints.Categories;

public class UpdateSubCategoryEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("/subcategories/{id:guid}", HandleAsync)
            .WithName("Atualizar uma subcategoria")
            .Produces<BaseResult>();

    private static async Task<IResult> HandleAsync(
        IMediator mediator,
        Guid id,
        UpdateSubCategoryRequest request)
    {
        var command = new UpdateSubCategoryCommand(
            id,
            request.Name,
            request.Description
        );

        var result = await mediator.Send(command);

        if (result.Success)
            return TypedResults.Ok(result);

        return TypedResults.BadRequest(result);
    }
}

public record UpdateSubCategoryRequest(
    string Name,
    string Description
);
