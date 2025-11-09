using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Categories.Commands;
using EChamado.Shared.Responses;
using Paramore.Brighter;

namespace EChamado.Server.Endpoints.Categories;

public class UpdateCategoryEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("/{id:guid}", HandleAsync)
            .WithName("Atualizar uma categoria")
            .Produces<BaseResult>();

    private static async Task<IResult> HandleAsync(
        IAmACommandProcessor commandProcessor,
        Guid id,
        UpdateCategoryRequest request)
    {
        var command = new UpdateCategoryCommand(
            id,
            request.Name,
            request.Description
        );

        var result = await commandProcessor.Send(command);

        if (result.Success)
            return TypedResults.Ok(result);

        return TypedResults.BadRequest(result);
    }
}

public record UpdateCategoryRequest(
    string Name,
    string Description
);
