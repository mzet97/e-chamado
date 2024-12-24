using EChamado.Application.Features.Users.ViewModels;
using EChamado.Core.Responses;
using MediatR;

namespace EChamado.Application.Features.Users.Queries;

public class GetByIdUserQuery : IRequest<BaseResult<ApplicationUserViewModel>>
{
    public Guid Id { get; set; }

    public GetByIdUserQuery(Guid id)
    {
        Id = id;
    }
}
