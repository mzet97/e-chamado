using EChamado.Server.Application.Common.Messaging;
using EChamado.Server.Application.UseCases.Users.ViewModels;
using EChamado.Shared.Responses;

namespace EChamado.Server.Application.UseCases.Users.Queries;

public class GetByEmailUserQuery : BrighterRequest<BaseResult<ApplicationUserViewModel>>
{
    public string Email { get; set; } = string.Empty;

    public GetByEmailUserQuery()
    {
    }

    public GetByEmailUserQuery(string email)
    {
        Email = email;
    }
}
