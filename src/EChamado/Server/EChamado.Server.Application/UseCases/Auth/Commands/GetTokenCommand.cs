using EChamado.Server.Application.Common.Messaging;
using EChamado.Shared.Responses;
using EChamado.Shared.ViewModels.Auth;

namespace EChamado.Server.Application.UseCases.Auth.Commands;

public class GetTokenCommand : BrighterRequest<BaseResult<LoginResponseViewModel>>
{
    public string Email { get; set; } = string.Empty;

    public GetTokenCommand()
    {
    }

    public GetTokenCommand(string email)
    {
        Email = email;
    }
}