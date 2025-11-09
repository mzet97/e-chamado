using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Categories.Commands;
using EChamado.Shared.Responses;
using Paramore.Brighter;

namespace EChamado.Server.Endpoints.Categories;

public class CreateCategoryEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/", HandleAsync)
            .WithName("Criar uma nova categoria")
            .Produces<BaseResult<Guid>>();

    private static async Task<IResult> HandleAsync(
        IAmACommandProcessor commandProcessor,
        CreateCategoryRequest request)
    {
        var command = new CreateCategoryCommand(
            request.Name,
            request.Description
        );

        var result = await commandProcessor.Send(command);

        if (result.Success)
            return TypedResults.Ok(result);

        return TypedResults.BadRequest(result);
    }
}

public record CreateCategoryRequest(
    string Name,
    string Description
);
