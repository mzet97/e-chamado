using EChamado.Server.Application.UseCases.Categories.Commands;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Endpoints.Categories;

public class UpdateCategoryEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("/{id:guid}", HandleAsync)
            .WithName("Atualizar uma categoria")
            .Produces<BaseResult>();

    private static async Task<IResult> HandleAsync(
        IMediator mediator,
        Guid id,
        UpdateCategoryRequest request)
    {
        var command = new UpdateCategoryCommand(
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

public record UpdateCategoryRequest(
    string Name,
    string Description
);
