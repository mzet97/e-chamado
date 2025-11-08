using EChamado.Server.Application.UseCases.Categories.ViewModels;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Categories.Queries;

public class GetAllCategoriesQuery : IRequest<BaseResultList<CategoryViewModel>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchText { get; set; }
}
