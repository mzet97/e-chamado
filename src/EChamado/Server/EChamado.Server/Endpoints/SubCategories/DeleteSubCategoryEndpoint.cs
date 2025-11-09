using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Categories.Commands;
using EChamado.Shared.Responses;
using Paramore.Brighter;

namespace EChamado.Server.Endpoints.SubCategories;

public class DeleteSubCategoryEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapDelete("/{id:guid}", HandleAsync)
            .WithName("Deletar uma subcategoria")
            .Produces<BaseResult>();

    private static async Task<IResult> HandleAsync(
        IAmACommandProcessor commandProcessor,
        Guid id)
    {
        var command = new DeleteSubCategoryCommand(id);
        var result = await commandProcessor.Send(command);

        if (result.Success)
            return TypedResults.Ok(result);

        return TypedResults.BadRequest(result);
    }
}
