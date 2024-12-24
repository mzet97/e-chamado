using EChamado.Api.Common.Api;
using EChamado.Application.Features.Departments.Commands;
using EChamado.Core.Responses;
using MediatR;

namespace EChamado.Api.Endpoints.Departments;

public class CreateDepartmentEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    => app.MapPost("/", HandleAsync)
        .WithName("Criar um novo departamento")
        .WithSummary("Criar um novo departamento")
        .WithDescription("Criar um novo departamento")
        .WithOrder(3)
        .Produces<BaseResult<Guid>>();

    private static async Task<IResult> HandleAsync(
        IMediator mediator,
        CreateDepartmentCommand command)
    {

        var result = await mediator.Send(command);

        if (result.Success)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.BadRequest(result);
    }
}