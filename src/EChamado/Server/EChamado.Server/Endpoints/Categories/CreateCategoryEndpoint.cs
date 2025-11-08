using EChamado.Server.Application.UseCases.Categories.Commands;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Endpoints.Categories;

public class CreateCategoryEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/", HandleAsync)
            .WithName("Criar uma nova categoria")
            .Produces<BaseResult<Guid>>();

    private static async Task<IResult> HandleAsync(
        IMediator mediator,
        CreateCategoryRequest request)
    {
        var command = new CreateCategoryCommand(
            request.Name,
            request.Description
        );

        var result = await mediator.Send(command);

        if (result.Success)
            return TypedResults.Ok(result);

        return TypedResults.BadRequest(result);
    }
}

public record CreateCategoryRequest(
    string Name,
    string Description
);
