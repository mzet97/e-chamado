using EChamado.Server.Application.UseCases.Categories.Queries;
using EChamado.Server.Application.UseCases.Categories.ViewModels;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.Categories;

public class GetAllCategoriesEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/", HandleAsync)
            .WithName("Buscar todas as categorias")
            .Produces<BaseResultList<CategoryViewModel>>();

    private static async Task<IResult> HandleAsync(
        IMediator mediator,
        [AsParameters] GetAllCategoriesParameters parameters)
    {
        var query = new GetAllCategoriesQuery
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

public class GetAllCategoriesParameters
{
    [FromQuery] public int PageNumber { get; set; } = 1;
    [FromQuery] public int PageSize { get; set; } = 10;
    [FromQuery] public string? SearchText { get; set; }
}
