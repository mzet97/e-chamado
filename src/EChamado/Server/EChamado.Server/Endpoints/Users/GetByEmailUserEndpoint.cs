using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Users.Queries;
using EChamado.Server.Application.UseCases.Users.ViewModels;
using EChamado.Server.Common.Api;
using EChamado.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using Paramore.Brighter;

namespace EChamado.Server.Endpoints.Users;

public class GetByEmailUserEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    => app.MapGet("/{email:alpha}", HandleAsync)
        .WithName("Obtem user pelo email")
        .WithSummary("Obtem user pelo email")
        .WithDescription("Obtem user pelo email")
        .WithOrder(3)
        .Produces<BaseResult<ApplicationUserViewModel>>();

    private static async Task<IResult> HandleAsync(
        IAmACommandProcessor commandProcessor,
        [FromRoute] string email)
    {
        var result = await commandProcessor.Send(new GetByEmailUserQuery(email));

        if (result.Success)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.BadRequest(result);
    }
}