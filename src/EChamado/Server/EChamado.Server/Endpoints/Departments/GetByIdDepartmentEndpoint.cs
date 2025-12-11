using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Departments.Queries;
using EChamado.Server.Application.UseCases.Departments.ViewModels;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using Paramore.Brighter;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.Departments;

public class GetByIdDepartmentEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    => app.MapGet("/{id:guid}", HandleAsync)
        .WithName("Obtem departamento pelo id")
        .WithSummary("Obtem departamento pelo id")
        .WithDescription("Obtem departamento pelo id")
        .WithOrder(2)
        .Produces<BaseResult<DepartmentViewModel>>();

    private static async Task<IResult> HandleAsync(
        Guid id,
        [FromServices] IAmACommandProcessor commandProcessor)
    {
        try
        {
            var query = new GetByIdDepartmentQuery(id);
            await commandProcessor.SendAsync(query);

            return query.Result.Success
                ? TypedResults.Ok(query.Result)
                : TypedResults.NotFound(query.Result);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(new BaseResult<DepartmentViewModel>(
                data: null,
                success: false,
                message: $"Erro interno: {ex.Message}"));
        }
    }
}