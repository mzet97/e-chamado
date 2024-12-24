using EChamado.Api.Common.Api;
using EChamado.Application.Features.Users.Queries;
using EChamado.Application.Features.Users.ViewModels;
using EChamado.Core.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Api.Endpoints.Users;

public class GetByIdUserEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    => app.MapGet("/{id:guid}", HandleAsync)
        .WithName("Obtem user pelo id")
        .WithSummary("Obtem user pelo id")
        .WithDescription("Obtem user pelo id")
        .WithOrder(2)
        .Produces<BaseResult<ApplicationUserViewModel>>();

    private static async Task<IResult> HandleAsync(
        IMediator mediator,
        [FromRoute] Guid id)
    {
        var result = await mediator.Send(new GetByIdUserQuery(id));

        if (result.Success)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.BadRequest(result);
    }
}