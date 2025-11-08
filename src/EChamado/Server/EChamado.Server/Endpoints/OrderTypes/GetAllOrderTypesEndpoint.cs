using EChamado.Server.Application.UseCases.OrderTypes.Queries;
using EChamado.Server.Application.UseCases.OrderTypes.ViewModels;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.OrderTypes;

public class GetAllOrderTypesEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/", HandleAsync)
            .WithName("Buscar todos os tipos de chamados")
            .Produces<BaseResultList<OrderTypeViewModel>>();

    private static async Task<IResult> HandleAsync(
        IMediator mediator,
        [AsParameters] GetAllOrderTypesParameters parameters)
    {
        var query = new GetAllOrderTypesQuery
        {
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize,
            SearchText = parameters.SearchText
        };

        var result = await mediator.Send(query);

        if (result.Success)
            return TypedResults.Ok(result);

        return TypedResults.BadRequest(result);
    }
}

public class GetAllOrderTypesParameters
{
    [FromQuery] public int PageNumber { get; set; } = 1;
    [FromQuery] public int PageSize { get; set; } = 10;
    [FromQuery] public string? SearchText { get; set; }
}
