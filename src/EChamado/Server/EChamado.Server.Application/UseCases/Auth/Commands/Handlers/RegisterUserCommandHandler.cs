using EChamado.Server.Application.UseCases.Auth.Notifications;
using EChamado.Server.Domain.Services.Interface;
using EChamado.Shared.Responses;
using EChamado.Shared.ViewModels.Auth;
using MediatR;
using System.Text;

namespace EChamado.Server.Application.UseCases.Auth.Commands.Handlers;

public class RegisterUserCommandHandler(
    IApplicationUserService applicationUserService,
    IMediator mediator
    ) : IRequestHandler<RegisterUserCommand, BaseResult<LoginResponseViewModel>>
{
    public async Task<BaseResult<LoginResponseViewModel>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
{
    var user = request.ToDomain();

    var resultCreateUser = await applicationUserService.CreateAsync(user, request.Password);

    if (resultCreateUser.Succeeded)
    {
        await applicationUserService.TrySignInAsync(user);
        return await mediator.Send(new GetTokenCommand { Email = request.Email });
    }

    var sb = new StringBuilder();
    foreach (var error in resultCreateUser.Errors)
    {
        sb.Append(error.Description);
    }

    await mediator.Publish(new RegisterUserNotification { Email = request.Email, Message = sb.ToString() });

    return new BaseResult<LoginResponseViewModel>(null, false, sb.ToString());
}
}
