using EChamado.Server.Application.UseCases.OrderTypes.ViewModels;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.OrderTypes.Queries;

public class GetAllOrderTypesQuery : IRequest<BaseResultList<OrderTypeViewModel>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchText { get; set; }
}
