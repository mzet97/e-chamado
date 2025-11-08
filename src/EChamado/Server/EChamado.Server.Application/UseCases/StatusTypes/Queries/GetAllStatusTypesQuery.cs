using EChamado.Server.Application.UseCases.StatusTypes.ViewModels;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.StatusTypes.Queries;

public class GetAllStatusTypesQuery : IRequest<BaseResultList<StatusTypeViewModel>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchText { get; set; }
}
