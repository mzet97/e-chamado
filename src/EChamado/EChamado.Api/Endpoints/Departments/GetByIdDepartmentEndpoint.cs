using EChamado.Api.Common.Api;
using EChamado.Application.Features.Departments.Queries;
using EChamado.Application.Features.Departments.ViewModels;
using EChamado.Core.Responses;
using MediatR;

namespace EChamado.Api.Endpoints.Departments;

public class GetByIdDepartmentEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    => app.MapGet("/{id}", HandleAsync)
        .WithName("Obtem departamento pelo id")
        .WithSummary("Obtem departamento pelo id")
        .WithDescription("Obtem departamento pelo id")
        .WithOrder(2)
        .Produces<BaseResult<DepartmentViewModel>>();

    private static async Task<IResult> HandleAsync(
        IMediator mediator, 
        Guid id)
    {
        var result = await mediator.Send(new GetByIdDepartmentQuery(id));

        if (result.Success)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.BadRequest(result);
    }
}