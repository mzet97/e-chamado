using EChamado.Shared.Responses;
using EChamado.Shared.ViewModels.Auth;
using MediatR;

namespace EChamado.Server.Application.UseCases.Auth.Commands;

public class GetTokenCommand :
    IRequest<BaseResult<LoginResponseViewModel>>
{
    public string Email { get; set; } = string.Empty;
}