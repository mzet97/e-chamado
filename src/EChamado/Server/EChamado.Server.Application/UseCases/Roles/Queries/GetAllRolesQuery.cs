using EChamado.Server.Application.UseCases.Roles.ViewModels;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Roles.Queries;

public class GetAllRolesQuery : IRequest<BaseResultList<RolesViewModel>>
{
}
