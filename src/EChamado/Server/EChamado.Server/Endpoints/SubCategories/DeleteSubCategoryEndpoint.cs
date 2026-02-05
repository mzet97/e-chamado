using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Categories.Commands;
using EChamado.Server.Endpoints.SubCategories.DTOs;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using Paramore.Brighter;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.SubCategories;

public class DeleteSubCategoryEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapDelete("/{id:guid}", HandleAsync)
            .WithName("Deletar uma subcategoria")
            .Produces<BaseResult>();

    private static async Task<IResult> HandleAsync(
        Guid id,
        [FromServices] IAmACommandProcessor commandProcessor)
    {
        try
        {
            var request = new DeleteSubCategoryRequest { Id = id };
            var command = request.ToCommand();
            await commandProcessor.SendAsync(command);

            var result = command.Result;

            if (result.Success)
                return TypedResults.Ok(result);

            return TypedResults.BadRequest(result);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(new BaseResult(
                success: false,
                message: $"Erro interno: {ex.Message}"));
        }
    }
}
