using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Roles.ViewModels;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.Roles.Queries;

public class GetAllRolesQuery : BrighterRequest<BaseResultList<RolesViewModel>>
{
}
