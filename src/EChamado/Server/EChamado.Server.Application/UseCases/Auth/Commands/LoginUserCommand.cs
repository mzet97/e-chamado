using EChamado.Server.Application.Common.Messaging;
using EChamado.Shared.Responses;
using EChamado.Shared.ViewModels.Auth;

namespace EChamado.Server.Application.UseCases.Auth.Commands;

public class LoginUserCommand : BrighterRequest<BaseResult<LoginResponseViewModel>>
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public LoginUserCommand()
    {
    }

    public LoginUserCommand(string email, string password)
    {
        Email = email;
        Password = password;
    }
}
