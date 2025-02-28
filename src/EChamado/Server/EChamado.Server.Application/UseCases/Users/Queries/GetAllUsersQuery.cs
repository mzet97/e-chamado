using EChamado.Server.Application.UseCases.Users.ViewModels;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Users.Queries;

public class GetAllUsersQuery : IRequest<BaseResultList<ApplicationUserViewModel>>
{
}
