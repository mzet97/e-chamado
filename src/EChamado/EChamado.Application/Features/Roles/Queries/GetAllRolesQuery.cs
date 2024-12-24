using EChamado.Application.Features.Roles.ViewModels;
using EChamado.Core.Responses;
using MediatR;

namespace EChamado.Application.Features.Roles.Queries;

public class GetAllRolesQuery : IRequest<BaseResultList<RolesViewModel>>
{
}
