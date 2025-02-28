using EChamado.Server.Application.UseCases.Users.ViewModels;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Users.Queries;

public class GetByIdUserQuery : IRequest<BaseResult<ApplicationUserViewModel>>
{
    public Guid Id { get; set; }

    public GetByIdUserQuery(Guid id)
    {
        Id = id;
    }
}
