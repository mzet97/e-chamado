using EChamado.Server.Application.UseCases.Categories.Commands;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Endpoints.Categories;

public class CreateSubCategoryEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/{categoryId:guid}/subcategories", HandleAsync)
            .WithName("Criar uma nova subcategoria")
            .Produces<BaseResult<Guid>>();

    private static async Task<IResult> HandleAsync(
        IMediator mediator,
        Guid categoryId,
        CreateSubCategoryRequest request)
    {
        var command = new CreateSubCategoryCommand(
            request.Name,
            request.Description,
            categoryId
        );

        var result = await mediator.Send(command);

        if (result.Success)
            return TypedResults.Ok(result);

        return TypedResults.BadRequest(result);
    }
}

public record CreateSubCategoryRequest(
    string Name,
    string Description
);
