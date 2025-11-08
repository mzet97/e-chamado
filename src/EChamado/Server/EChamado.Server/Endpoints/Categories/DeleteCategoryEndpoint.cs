using EChamado.Server.Application.UseCases.Categories.Commands;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Endpoints.Categories;

public class DeleteCategoryEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapDelete("/{id:guid}", HandleAsync)
            .WithName("Deletar uma categoria")
            .Produces<BaseResult>();

    private static async Task<IResult> HandleAsync(
        IMediator mediator,
        Guid id)
    {
        var command = new DeleteCategoryCommand(id);
        var result = await mediator.Send(command);

        if (result.Success)
            return TypedResults.Ok(result);

        return TypedResults.BadRequest(result);
    }
}
