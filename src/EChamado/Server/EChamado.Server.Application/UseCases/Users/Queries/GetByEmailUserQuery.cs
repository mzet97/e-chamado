using EChamado.Server.Application.UseCases.Users.ViewModels;
using EChamado.Shared.Responses;
using MediatR;

namespace EChamado.Server.Application.UseCases.Users.Queries;

public class GetByEmailUserQuery : IRequest<BaseResult<ApplicationUserViewModel>>
{
    public string Email { get; set; } = string.Empty;

    public GetByEmailUserQuery(string email)
    {
        Email = email;
    }
}
