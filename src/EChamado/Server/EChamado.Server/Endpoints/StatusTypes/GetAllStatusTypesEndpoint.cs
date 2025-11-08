using EChamado.Server.Application.UseCases.StatusTypes.Queries;
using EChamado.Server.Application.UseCases.StatusTypes.ViewModels;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.StatusTypes;

public class GetAllStatusTypesEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/", HandleAsync)
            .WithName("Buscar todos os status de chamados")
            .Produces<BaseResultList<StatusTypeViewModel>>();

    private static async Task<IResult> HandleAsync(
        IMediator mediator,
        [AsParameters] GetAllStatusTypesParameters parameters)
    {
        var query = new GetAllStatusTypesQuery
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

public class GetAllStatusTypesParameters
{
    [FromQuery] public int PageNumber { get; set; } = 1;
    [FromQuery] public int PageSize { get; set; } = 10;
    [FromQuery] public string? SearchText { get; set; }
}
