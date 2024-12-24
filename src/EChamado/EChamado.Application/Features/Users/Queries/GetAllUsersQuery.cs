using EChamado.Application.Features.Users.ViewModels;
using EChamado.Core.Domains.Identities;
using EChamado.Core.Responses;
using MediatR;

namespace EChamado.Application.Features.Users.Queries;

public class GetAllUsersQuery : IRequest<BaseResultList<ApplicationUserViewModel>>
{
}
