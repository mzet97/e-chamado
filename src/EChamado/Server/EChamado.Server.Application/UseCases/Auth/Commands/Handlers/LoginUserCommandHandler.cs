using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Application.UseCases.Auth.Notifications;
using EChamado.Server.Domain.Services.Interface;
using EChamado.Shared.Responses;
using EChamado.Shared.ViewModels.Auth;
using Paramore.Brighter;

namespace EChamado.Server.Application.UseCases.Auth.Commands.Handlers;

public class LoginUserCommandHandler(
    IApplicationUserService applicationUserService,
    IAmACommandProcessor commandProcessor)
    : RequestHandlerAsync<LoginUserCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<LoginUserCommand> HandleAsync(LoginUserCommand command, CancellationToken cancellationToken = default)
    {
        var result = await applicationUserService.PasswordSignInAsync(command.Email, command.Password, false, false);

        if (result.Succeeded)
        {
            var tokenResult = await commandProcessor.SendAsync(new GetTokenCommand { Email = command.Email }, cancellationToken: cancellationToken);
            command.Result = tokenResult.Result;
            return await base.HandleAsync(command, cancellationToken);
        }
        else if (result.IsLockedOut)
        {
            await commandProcessor.PublishAsync(new LoginUserNotification { Email = command.Email, Message = "Falha: Login bloqueado" }, cancellationToken: cancellationToken);
            command.Result = new BaseResult<LoginResponseViewModel>(null, false, "Falha: Login bloqueado");
            return await base.HandleAsync(command, cancellationToken);
        }

        await commandProcessor.PublishAsync(new LoginUserNotification { Email = command.Email, Message = "Falha: Erro ao fazer login" }, cancellationToken: cancellationToken);

        command.Result = new BaseResult<LoginResponseViewModel>(null, false, "Falha: Erro ao fazer login");
        return await base.HandleAsync(command, cancellationToken);
    }
}
