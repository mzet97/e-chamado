using EChamado.Server.Application.UseCases.Categories.ViewModels;
using EChamado.Server.Application.UseCases.SubCategories.Queries;
using EChamado.Shared.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EChamado.Server.Endpoints.SubCategories;

public class SearchSubCategoriesEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/", HandleAsync)
            .WithName("Buscar subcategorias")
            .Produces<BaseResultList<SubCategoryViewModel>>();

    private static async Task<IResult> HandleAsync(
        IMediator mediator,
        [AsParameters] SearchSubCategoriesParameters parameters)
    {
        var query = new SearchSubCategoriesQuery
        {
            Id = parameters.Id,
            Name = parameters.Name ?? string.Empty,
            Description = parameters.Description ?? string.Empty,
            CategoryId = parameters.CategoryId,
            CreatedAt = parameters.CreatedAt,
            UpdatedAt = parameters.UpdatedAt,
            DeletedAt = parameters.DeletedAt,
            Order = parameters.Order,
            PageIndex = parameters.PageIndex,
            PageSize = parameters.PageSize
        };

        var result = await mediator.Send(query);

        if (result.Success)
            return TypedResults.Ok(result);

        return TypedResults.BadRequest(result);
    }
}

public class SearchSubCategoriesParameters
{
    [FromQuery] public Guid Id { get; set; }
    [FromQuery] public string? Name { get; set; }
    [FromQuery] public string? Description { get; set; }
    [FromQuery] public Guid? CategoryId { get; set; }
    [FromQuery] public DateTime CreatedAt { get; set; }
    [FromQuery] public DateTime UpdatedAt { get; set; }
    [FromQuery] public DateTime? DeletedAt { get; set; }
    [FromQuery] public string? Order { get; set; }
    [FromQuery] public int PageIndex { get; set; } = 1;
    [FromQuery] public int PageSize { get; set; } = 10;
}
