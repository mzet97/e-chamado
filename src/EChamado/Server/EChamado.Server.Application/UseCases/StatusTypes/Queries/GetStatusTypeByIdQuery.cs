using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.StatusTypes.ViewModels;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.StatusTypes.Queries;

public class GetStatusTypeByIdQuery : BrighterRequest<BaseResult<StatusTypeViewModel>>
{
    public Guid Id { get; set; }

    public GetStatusTypeByIdQuery()
    {
    }

    public GetStatusTypeByIdQuery(Guid id)
    {
        Id = id;
    }
}
