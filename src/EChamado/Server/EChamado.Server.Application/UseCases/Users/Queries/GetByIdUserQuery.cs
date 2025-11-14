using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Users.ViewModels;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.Users.Queries;

public class GetByIdUserQuery : BrighterRequest<BaseResult<ApplicationUserViewModel>>
{
    public Guid Id { get; set; }

    public GetByIdUserQuery()
    {
    }

    public GetByIdUserQuery(Guid id)
    {
        Id = id;
    }
}
