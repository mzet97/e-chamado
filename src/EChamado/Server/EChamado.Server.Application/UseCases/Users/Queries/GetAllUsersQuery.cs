using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Users.ViewModels;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.Users.Queries;

public class GetAllUsersQuery : BrighterRequest<BaseResultList<ApplicationUserViewModel>>
{
}
