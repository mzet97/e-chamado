using EChamado.Server.Application.UseCases.Auth.Notifications;
using EChamado.Server.Domain.Services.Interface;
using EChamado.Shared.Responses;
using EChamado.Shared.ViewModels.Auth;
using MediatR;

namespace EChamado.Server.Application.UseCases.Auth.Commands.Handlers;

public class LoginUserCommandHandler(
    IApplicationUserService applicationUserService,
    IMediator mediator)
    : IRequestHandler<LoginUserCommand, BaseResult<LoginResponseViewModel>>
{
    public async Task<BaseResult<LoginResponseViewModel>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
{
    var result = await applicationUserService.PasswordSignInAsync(request.Email, request.Password, false, false);

    if (result.Succeeded)
    {
        return await mediator.Send(new GetTokenCommand { Email = request.Email });
    }
    else if (result.IsLockedOut)
    {
        await mediator.Publish(new LoginUserNotification { Email = request.Email, Message = "Falha: Login bloqueado" });
        return new BaseResult<LoginResponseViewModel>(null, false, "Falha: Login bloqueado");
    }

    await mediator.Publish(new LoginUserNotification { Email = request.Email, Message = "Falha: Erro ao fazer login" });

    return new BaseResult<LoginResponseViewModel>(null, false, "Falha: Erro ao fazer login");
}
}
