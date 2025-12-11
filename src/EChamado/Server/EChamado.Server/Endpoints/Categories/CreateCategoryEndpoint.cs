using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Categories.Commands;
using EChamado.Server.Endpoints.Categories.DTOs;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using Paramore.Brighter;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.Categories;

public class CreateCategoryEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/", HandleAsync)
            .WithName("Criar uma nova categoria")
            .Produces<BaseResult<Guid>>();

    private static async Task<IResult> HandleAsync(
        [FromServices] IAmACommandProcessor commandProcessor,
        [FromBody] CreateCategoryRequest request)
    {
        try
        {
            var command = request.ToCommand();
            await commandProcessor.SendAsync(command);

            var result = command.Result;

            if (result.Success)
                return TypedResults.Ok(result);

            return TypedResults.BadRequest(result);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(new BaseResult<Guid>(
                data: Guid.Empty,
                success: false,
                message: $"Erro interno: {ex.Message}"));
        }
    }
}
