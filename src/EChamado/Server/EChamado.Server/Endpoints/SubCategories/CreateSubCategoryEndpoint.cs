using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Categories.Commands;
using EChamado.Shared.Responses;
using Paramore.Brighter;

namespace EChamado.Server.Endpoints.SubCategories;

public class CreateSubCategoryEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/", HandleAsync)
            .WithName("Criar uma nova subcategoria")
            .Produces<BaseResult<Guid>>();

    private static async Task<IResult> HandleAsync(
        IAmACommandProcessor commandProcessor,
        CreateSubCategoryRequest request)
    {
        var command = new CreateSubCategoryCommand(
            request.Name,
            request.Description,
            request.CategoryId
        );

        var result = await commandProcessor.Send(command);

        if (result.Success)
            return TypedResults.Ok(result);

        return TypedResults.BadRequest(result);
    }
}

public record CreateSubCategoryRequest(
    string Name,
    string Description,
    Guid CategoryId
);
