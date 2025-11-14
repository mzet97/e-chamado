using EChamado.Server.Application.Common.Behaviours;
using EChamado.Server.Application.UseCases.Auth.Notifications;
using EChamado.Server.Domain.Services.Interface;
using EChamado.Shared.Responses;
using EChamado.Shared.ViewModels.Auth;
using Paramore.Brighter;
using System.Text;

namespace EChamado.Server.Application.UseCases.Auth.Commands.Handlers;

public class RegisterUserCommandHandler(
    IApplicationUserService applicationUserService,
    IAmACommandProcessor commandProcessor
    ) : RequestHandlerAsync<RegisterUserCommand>
{
    [RequestLogging(0, HandlerTiming.Before)]
    [RequestValidation(1, HandlerTiming.Before)]
    public override async Task<RegisterUserCommand> HandleAsync(RegisterUserCommand command, CancellationToken cancellationToken = default)
    {
        var user = command.ToDomain();

        var resultCreateUser = await applicationUserService.CreateAsync(user, command.Password);

        if (resultCreateUser.Succeeded)
        {
            await applicationUserService.TrySignInAsync(user);
            var tokenCommand = new GetTokenCommand { Email = command.Email };
            await commandProcessor.SendAsync(tokenCommand, cancellationToken: cancellationToken);
            command.Result = tokenCommand.Result;
            return await base.HandleAsync(command, cancellationToken);
        }

        var sb = new StringBuilder();
        foreach (var error in resultCreateUser.Errors)
        {
            sb.Append(error.Description);
        }

        await commandProcessor.PublishAsync(new RegisterUserNotification { Email = command.Email, Message = sb.ToString() }, cancellationToken: cancellationToken);

        command.Result = new BaseResult<LoginResponseViewModel>(null, false, sb.ToString());
        return await base.HandleAsync(command, cancellationToken);
    }
}
